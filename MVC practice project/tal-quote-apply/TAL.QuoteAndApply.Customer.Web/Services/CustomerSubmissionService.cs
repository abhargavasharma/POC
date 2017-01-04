using System;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Services.Converters;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Http.Exceptions;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public interface ICustomerSubmissionService
    {
        bool SavePurchaseRequestAndSubmitPolicy(string quoteReference, int riskId, PurchaseRequest purchaseRequest);
    }

    public class CustomerSubmissionService : ICustomerSubmissionService
    {
        private readonly IBeneficiaryDetailsRequestConverter _beneficiaryModelMapper;
        private readonly IBeneficiaryDetailsService _beneficiaryDetailsService;
        private readonly IUpdateRiskService _updateRiskService;
        private readonly IPersonalDetailsResultConverter _personalDetailsResultConverter;
        private readonly IPolicyDeclarationService _policyDeclarationService;
        private readonly ICreditCardPaymentViewModelConverter _creditCardPaymentViewModelConverter;
        private readonly IPaymentOptionService _paymentOptionService;
        private readonly IDirectDebitPaymentViewModelConverter _directDebitPaymentViewModelConverter;

        private readonly IRaisePolicyService _raisePolicyService;
        private readonly ILoggingService _loggingService;
        
        public CustomerSubmissionService(IBeneficiaryDetailsRequestConverter beneficiaryModelMapper, 
            IBeneficiaryDetailsService beneficiaryDetailsService, 
            IUpdateRiskService updateRiskService, 
            IPersonalDetailsResultConverter personalDetailsResultConverter, 
            IPolicyDeclarationService policyDeclarationService,
            ICreditCardPaymentViewModelConverter creditCardPaymentViewModelConverter,
            IPaymentOptionService paymentOptionService,
            IDirectDebitPaymentViewModelConverter directDebitPaymentViewModelConverter, 
            IRaisePolicyService raisePolicyService, 
            ILoggingService loggingService)
        {
            _beneficiaryModelMapper = beneficiaryModelMapper;
            _beneficiaryDetailsService = beneficiaryDetailsService;
            _updateRiskService = updateRiskService;
            _personalDetailsResultConverter = personalDetailsResultConverter;
            _policyDeclarationService = policyDeclarationService;
            _creditCardPaymentViewModelConverter = creditCardPaymentViewModelConverter;
            _paymentOptionService = paymentOptionService;
            _directDebitPaymentViewModelConverter = directDebitPaymentViewModelConverter;
            _raisePolicyService = raisePolicyService;
            _loggingService = loggingService;
        }

        public bool SavePurchaseRequestAndSubmitPolicy(string quoteReference, int riskId, PurchaseRequest purchaseRequest)
        {
            UpdatePersonalDetails(riskId, purchaseRequest);
            UpdateDeclaration(quoteReference, purchaseRequest);
            UpdateBeneficiaries(riskId, purchaseRequest);

            if (purchaseRequest.PaymentOptions.IsDirectDebitSelected)
            {
                UpdateDirectDebit(purchaseRequest, quoteReference, riskId);
            }
            else if (purchaseRequest.PaymentOptions.IsCreditCardSelected)
            {
                try
                {
                    UpdateCreditCard(purchaseRequest, quoteReference, riskId);
                }
                catch (ThirdPartyServiceException ex)
                {
                    _loggingService.Error("The credit card details could not be successfully validated as the card system is down", ex);
                }   
            }

            var raiseResponse = _raisePolicyService.PostPolicy(quoteReference);

            if (raiseResponse.SubmittedSuccessfully)
            {
                return true;
            }

            _loggingService.Error($"Customer submission failure: {Environment.NewLine} {raiseResponse.ToJson()}");
            return false;
        }

        private void UpdateBeneficiaries(int riskId, PurchaseRequest purchaseRequest)
        {
            _beneficiaryDetailsService.UpdateLprForRisk(riskId, purchaseRequest.NominateLpr);

            foreach (var b in purchaseRequest.Beneficiaries)
            {
                _beneficiaryDetailsService.CreateOrUpdateBeneficiary(_beneficiaryModelMapper.From(b), riskId);
            }
        }

        private void UpdateDeclaration(string quoteReference, PurchaseRequest purchaseRequest)
        {
            _policyDeclarationService.UpdateDeclaration(quoteReference, purchaseRequest.DeclarationAgree);
        }

        private void UpdatePersonalDetails(int riskId, PurchaseRequest purchaseRequest)
        {
            _updateRiskService.UpdateRiskPersonalDetails(
                _personalDetailsResultConverter.From(purchaseRequest.PersonalDetails, riskId, purchaseRequest.DncSelection));
        }

        private void UpdateCreditCard(PurchaseRequest purchaseRequest, string quoteReferenceNumber, int riskId)
        {
            //update credit card
            var creditCardModel = _creditCardPaymentViewModelConverter.From(purchaseRequest.PaymentOptions.CreditCardPayment);
            _paymentOptionService.AssignCreditCardPayment(quoteReferenceNumber, riskId, creditCardModel);
        }

        private void UpdateDirectDebit(PurchaseRequest purchaseRequest, string quoteReferenceNumber, int riskId)
        {
            var directDebitModel = _directDebitPaymentViewModelConverter.From(purchaseRequest.PaymentOptions.DirectDebitPayment);
            _paymentOptionService.AssignDirectDebitPayment(quoteReferenceNumber, riskId, directDebitModel);
        }
    }
}
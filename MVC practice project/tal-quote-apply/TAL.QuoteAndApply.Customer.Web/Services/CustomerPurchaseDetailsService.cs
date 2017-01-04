using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public interface ICustomerPurchaseDetailsService
    {
        PurchaseAndPremiumResponse GetPurchaseForQuote(string quoteReference);
    }

    public class CustomerPurchaseDetailsService : ICustomerPurchaseDetailsService
    {
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IBeneficiaryDetailsService _beneficiaryDetailsService;
        private readonly IBeneficiaryDetailsRequestConverter _beneficiaryModelMapper;
        private readonly IPersonalDetailsResultConverter _personalDetailsResultConverter;
        private readonly IRiskPersonalDetailsProvider _riskPersonalDetailsProvider;
        private readonly IPaymentOptionService _paymentOptionService;
        private readonly IPaymentOptionsViewModelConverter _paymentOptionsViewModelConverter;

        public CustomerPurchaseDetailsService(
            IBeneficiaryDetailsService beneficiaryDetailsService, 
            IPolicyOverviewProvider policyOverviewProvider, 
            IBeneficiaryDetailsRequestConverter beneficiaryModelMapper, 
            IPersonalDetailsResultConverter personalDetailsResultConverter,
            IRiskPersonalDetailsProvider riskPersonalDetailsProvider,
            IPaymentOptionService paymentOptionService,
            IPaymentOptionsViewModelConverter paymentOptionsViewModelConverter)
        {
            _beneficiaryDetailsService = beneficiaryDetailsService;
            _policyOverviewProvider = policyOverviewProvider;
            _beneficiaryModelMapper = beneficiaryModelMapper;
            _personalDetailsResultConverter = personalDetailsResultConverter;
            _riskPersonalDetailsProvider = riskPersonalDetailsProvider;
            _paymentOptionService = paymentOptionService;
            _paymentOptionsViewModelConverter = paymentOptionsViewModelConverter;
        }

        public PurchaseAndPremiumResponse GetPurchaseForQuote(string quoteReference)
        {
            var policy = _policyOverviewProvider.GetFor(quoteReference);
            var riskPurchaseResponses = new List<PurchaseResponse>();

            foreach (var risk in policy.Risks)
            {
                var riskId = risk.RiskId;

                var paymentOptions = _paymentOptionService.GetCurrentPaymentOptions(quoteReference, riskId);
                var paymentOptionViewModel = _paymentOptionsViewModelConverter.From(paymentOptions);

                var purchaseViewModel = new PurchaseResponse
                {
                    RiskId = riskId,
                    Beneficiaries =
                        _beneficiaryModelMapper.FromList(
                            _beneficiaryDetailsService.GetBeneficiariesForRisk(riskId).ToList()),
                    DisclosureNotes = new PolicyNoteResultViewModel(),
                    NominateLpr = _beneficiaryDetailsService.GetLprForRisk(riskId),
                    DeclarationAgree = policy.DeclarationAgree,
                    PersonalDetails =
                        _personalDetailsResultConverter.From(
                            _riskPersonalDetailsProvider.GetFor(riskId),
                            policy.Risks.FirstOrDefault(r => r.RiskId == riskId)),
                    PaymentOptions = paymentOptionViewModel,
                    DncSelection = false //Marketing opt out will always appear as unchecked in customer portal
                };
                riskPurchaseResponses.Add(purchaseViewModel);
            }

            return new PurchaseAndPremiumResponse
            {
                RiskPurchaseResponses = riskPurchaseResponses,
                PaymentFrequency = policy.PremiumFrequency.ToString(),
                TotalPremium = policy.Premium
            };
        }
    }
}

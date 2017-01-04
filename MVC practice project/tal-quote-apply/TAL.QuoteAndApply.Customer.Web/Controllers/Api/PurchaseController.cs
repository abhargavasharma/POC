using System;
using System.Linq;
using System.Web.Http;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [WebApiQuoteSessionRequired]
    [RoutePrefix("api/purchase")]
    public class PurchaseController : BaseCustomerPortalApiController
    {
        private readonly ICustomerPurchaseDetailsService _customerPurchaseDetailsService;
        private readonly ICustomerPurchaseValidationService _customerPurchaseValidationService;
        private readonly ICustomerSubmissionService _customerSubmissionService;
        private readonly IBeneficiaryDetailsService _beneficiaryDetailsService;
        private readonly ICustomerReferralService _customerReferralService;
        private readonly ILoggingService _loggingService;


        public PurchaseController(IQuoteSessionContext quoteSessionContext, 
            IPolicyOverviewProvider policyOverviewProvider, 
            ICustomerPurchaseDetailsService customerPurchaseDetailsService, 
            ICustomerPurchaseValidationService customerPurchaseValidationService,
            ICustomerSubmissionService customerSubmissionService, 
            IBeneficiaryDetailsService beneficiaryDetailsService, 
            ICustomerReferralService customerReferralService, ILoggingService loggingService) 
            : base(quoteSessionContext, policyOverviewProvider)
        {
            
            _customerPurchaseDetailsService = customerPurchaseDetailsService;
            _customerPurchaseValidationService = customerPurchaseValidationService;
            _customerSubmissionService = customerSubmissionService;
            _beneficiaryDetailsService = beneficiaryDetailsService;
            _customerReferralService = customerReferralService;
            _loggingService = loggingService;
        }

        [HttpGet]
        public IHttpActionResult GetPurchaseDetails()
        {
            var quoteReference = _quoteSessionContext.QuoteSession.QuoteReference;
            var retVal = _customerPurchaseDetailsService.GetPurchaseForQuote(quoteReference);

            return Ok(retVal);
        }

        /// <summary>
        /// submit purchase form including personal details, paymentOptions and beneficiaries
        /// </summary>        
        [HttpPost, Route("risk/{riskId:int}")]
        public IHttpActionResult PostPurchaseDetails(int riskId, [FromBody] PurchaseRequest purchaseRequest)
        {
            //TODO: remove this try/catch and isolate area of failure (e.g. SavePurchaseRequestAndSubmitPolicy)
            try
            {
                if (!IsRiskValidForApplicationSession(riskId))
                {
                    return BadRequest();
                }

                var result = _customerPurchaseValidationService.ValidatePurchaseForSave(purchaseRequest);
                UpdateModelState(result);

                if (!ModelState.IsValid)
                {
                    return new InvalidModelStateActionResult(ModelState);
                }

                var quoteReference = _quoteSessionContext.QuoteSession.QuoteReference;

                //update personal details and beneficiaries
                var submissionResult = _customerSubmissionService.SavePurchaseRequestAndSubmitPolicy(quoteReference, riskId, purchaseRequest);

                string redirectUrl;
                if (submissionResult)
                {
                    redirectUrl = Url.Route("Default", new { Controller = "Confirmation", Action = "Index" });
                }
                else
                {
                    _customerReferralService.SetPolicyAsCustomerReferral(quoteReference);
                    redirectUrl = Url.Route("Default", new { Controller = "Submission", Action = "Index" });
                }

                return new RedirectActionResult(redirectUrl);
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex);
                ModelState.AddModelError("SubmissionFailure", "SubmissionFailure");
                return new InvalidModelStateActionResult(ModelState);
            }
        }

        [HttpDelete, Route("risk/{riskId:int}/beneficiaries/{beneficiaryId:int}")]
        public IHttpActionResult RemoveBeneficiary(int riskId, int beneficiaryId)
        {
            _beneficiaryDetailsService.RemoveBeneficiary(riskId, beneficiaryId);
            return Ok();
        }

        private void UpdateModelState(CustomerPurchaseValidationModel purchaseModel)
        {
            var result = purchaseModel.Beneficiaries.ToArray();
            for (var idx = 0; idx < result.Length; idx++)
            {
                var riskBeneficiaryValidationModel = result[idx];
                if (!riskBeneficiaryValidationModel.IsValid)
                {
                    foreach (var validationError in riskBeneficiaryValidationModel.ValidationErrors)
                    {
                        foreach (var messages in validationError.Messages)
                        {
                            ModelState.AddModelError(string.Format("purchaseRequest.beneficiaries[{0}].{1}", idx, validationError.Key), messages);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(purchaseModel.PaymentErrorMessage))
            {
                ModelState.AddModelError("MultiplePaymentTypes", purchaseModel.PaymentErrorMessage);
            }
        }
    }
}
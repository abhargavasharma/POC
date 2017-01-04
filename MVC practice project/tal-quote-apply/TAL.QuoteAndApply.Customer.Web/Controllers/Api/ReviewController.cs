using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.ServiceLayer.Validation;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [WebApiQuoteSessionRequired]
    [RoutePrefix("api/review")]
    public class ReviewController : BaseCustomerPortalApiController
    {
        private readonly ICustomerPolicyStateService _customerPolicyStateService;
        private readonly IUpdatePlanService _updatePlanService;
        private readonly ICustomerReviewValidationService _customerReviewValidation;
		private readonly IRiskUnderwritingQuestionService _riskUnderwritingQuestionService;
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;
        private readonly ICustomerReferralService _customerReferralService;

        public ReviewController(ICustomerPolicyStateService customerPolicyStateService,
            IQuoteSessionContext quoteSessionContext, IUpdatePlanService updatePlanService,
            ICustomerReviewValidationService customerReviewValidation,
            IRiskUnderwritingQuestionService riskUnderwritingQuestionService,
            IPolicyOverviewProvider policyOverviewProvider,
            IPolicyPremiumCalculation policyPremiumCalculation, 
            ICustomerReferralService customerReferralService)
            : base(quoteSessionContext, policyOverviewProvider)
        {
            _customerPolicyStateService = customerPolicyStateService;
            _updatePlanService = updatePlanService;
            _customerReviewValidation = customerReviewValidation;
            _riskUnderwritingQuestionService = riskUnderwritingQuestionService;
            _policyPremiumCalculation = policyPremiumCalculation;
            _customerReferralService = customerReferralService;
        }

        [HttpGet, Route("risk/{riskId:int}")]
        public IHttpActionResult GetRiskReview(int riskId)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            _customerPolicyStateService.EnsureSamePremiumTypesAcrossPlans(_quoteSessionContext.QuoteSession.QuoteReference, riskId);
            var response = _customerPolicyStateService.GetRiskPolicyStatus(_quoteSessionContext.QuoteSession.QuoteReference, riskId);
            return Ok(response);
        }


        [HttpPost, Route("risk/{riskId:int}")]
        public IHttpActionResult SwitchQuestionChoice(int riskId, UpdateQuestionRequest updateQuestionRequest)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            _riskUnderwritingQuestionService.AnswerQuestionAndSync(
                riskId,
                updateQuestionRequest.QuestionId,
                updateQuestionRequest.SelectedAnswers.Select(
                    a => new UnderwritingAnswer(a.Id, a.Text)).ToList());

            //TODO: returning new state which will call underwriting again. Make more efficient by using the result from AnswerQuestion above
            _policyPremiumCalculation.CalculateAndSavePolicy(_quoteSessionContext.QuoteSession.QuoteReference);
            var response = _customerPolicyStateService.GetRiskPolicyStatus(_quoteSessionContext.QuoteSession.QuoteReference, riskId);
            return Ok(response);

        }

        [HttpPost, Route("risk/{riskId:int}/PremiumType")]
        public IHttpActionResult SetPremiumType(int riskId, PremiumTypeUpdateRequest premiumTypeUpdate)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var errors = _customerReviewValidation.ValidatePremiumType(_quoteSessionContext.QuoteSession.QuoteReference, riskId,
                premiumTypeUpdate.PremiumType);

            if (errors.Any())
            {
                //Seriously don't expect to get in here if going by the client since invalid Premium Type options should be unselectable on front end. This is more to protect API
                ApplyErrorsToModelState(errors);
                return new InvalidModelStateActionResult(ModelState);
            }

            _updatePlanService.UpdatePremiumTypeOnAllPlans(_quoteSessionContext.QuoteSession.QuoteReference, riskId, premiumTypeUpdate.PremiumType);
            var response = _customerPolicyStateService.GetRiskPolicyStatus(_quoteSessionContext.QuoteSession.QuoteReference, riskId);
            return Ok(response);
        }

        [HttpGet, Route("risk/{riskId:int}/validate")]
        public IHttpActionResult ValidateReview(int riskId)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            _customerReviewValidation.ValidateReview(QuoteReferenceNumber, riskId, ModelState);

            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            //If valid then redirect to either purchase or refer page
            var riskPolicyStatus = _customerPolicyStateService.GetRiskPolicyStatus(_quoteSessionContext.QuoteSession.QuoteReference, riskId);

            string redirectUrl;

            switch (riskPolicyStatus.ReviewWorkflowStatus)
            {
                case ReviewWorkflowStatus.Accept:
                    redirectUrl = Url.Route("Default", new {Controller = "Purchase", Action = "Index"});
                    break;
                default:
                    _customerReferralService.SetPolicyAsCustomerReferral(_quoteSessionContext.QuoteSession.QuoteReference);
                    redirectUrl = Url.Route("Default", new {Controller = "Submission", Action = "Index"});
                    break;
            }

            return new RedirectActionResult(redirectUrl);

        }

        private void ApplyErrorsToModelState(IEnumerable<ValidationError> errors)
        {
            foreach (var brokenRule in errors)
            {
                switch (brokenRule.Key)
                {
                    case ValidationKey.EligiblePremiumType:
                        ModelState.AddModelError("premiumType", brokenRule.Message);
                        break;
                }
            }

        }

    }
}
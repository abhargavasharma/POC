using System;
using System.Linq;
using System.Web.Http;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.Customer.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.Web.Shared.Converters;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [WebApiQuoteSessionRequired]
    [RoutePrefix("api/coverselection")]
    public class CoverSelectionController : BaseCustomerPortalApiController
    {
        private readonly IPolicyInitialisationMetadataService _policyInitialisationMetadataService;
        private readonly IPlanDetailsService _planDetailsService;
        private readonly IPlanStateParamFactory _planStateParamFactory;
        private readonly IUpdatePlanService _updatePlanService;
        private readonly IActivePlanValidationService _activePlanValidationService;
        private readonly IPlanRiderService _planRiderService;
        private readonly IPlanAutoUpdateService _planAutoUpdateService;
        private readonly IErrorsAndWarningsConverter _errorsAndWarningsConverter;
        private readonly ICustomerPolicyStateService _customerPolicyStateService;
        private readonly IRiskUnderwritingService _riskUnderwritingService;


        public CoverSelectionController(IPlanDetailsService planDetailsService, IQuoteSessionContext quoteSessionContext,
            IPlanStateParamFactory planStateParamFactory, IUpdatePlanService updatePlanService,
            IActivePlanValidationService activePlanValidationService, IPlanRiderService planRiderService,
            IPlanAutoUpdateService planAutoUpdateService,
            IPolicyInitialisationMetadataService policyInitialisationMetadataService,
            IErrorsAndWarningsConverter errorsAndWarningsConverter, IPolicyOverviewProvider policyOverviewProvider,
            ICustomerPolicyStateService customerPolicyStateService, IRiskUnderwritingService riskUnderwritingService)
            : base(quoteSessionContext, policyOverviewProvider)
        {
            _planDetailsService = planDetailsService;
            _planStateParamFactory = planStateParamFactory;
            _updatePlanService = updatePlanService;
            _activePlanValidationService = activePlanValidationService;
            _planRiderService = planRiderService;
            _planAutoUpdateService = planAutoUpdateService;
            _policyInitialisationMetadataService = policyInitialisationMetadataService;
            _errorsAndWarningsConverter = errorsAndWarningsConverter;
            _customerPolicyStateService = customerPolicyStateService;
            _riskUnderwritingService = riskUnderwritingService;
        }

        [HttpGet]
        [Route("risk/{riskId}")]
        public IHttpActionResult GetPlanForRisk(int riskId)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var planDetailsForRisk = GetPlanDetailsForRisk(riskId);
            return Ok(planDetailsForRisk);
        }


        [HttpPost]
        [Route("risk/{riskId}")]
        public IHttpActionResult UpdatePlan(int riskId, UpdatePlanRequest planRequest)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var quoteReferenceNumber = _quoteSessionContext.QuoteSession.QuoteReference;

            var planState = GetPlanState(riskId, planRequest, quoteReferenceNumber);

            var validationResult = _activePlanValidationService.ValidateCurrentActivePlan(planState);

            if (validationResult.HasErrorsOrWarnings)
            {
                if (validationResult.Errors != null)
                {
                    _errorsAndWarningsConverter.MapPlanValidationsToModelState(ModelState, validationResult.Errors,
                        planState.PlanCode);
                }
                if (validationResult.Warnings != null)
                {
                    _errorsAndWarningsConverter.MapPlanValidationsToModelState(ModelState, validationResult.Warnings,
                        planState.PlanCode);
                }
                return new InvalidModelStateActionResult(ModelState);
            }

            UpdatePlan(planState, quoteReferenceNumber);

            if (planRequest.IncludeReviewInfoInResponse != null && planRequest.IncludeReviewInfoInResponse.Value)
            {
                /*TODO: Consider moving Review stuff to it's own endpoint on Review controller.
                (Code for Select Cover and Review used to be the same so used same endpoint, but now slowly require different logic) */
                var retValWithReviewInfo = _customerPolicyStateService.GetRiskPolicyStatus(quoteReferenceNumber, riskId);
                _customerPolicyStateService.SyncWorkflowInterviewStatus(riskId, retValWithReviewInfo.ReviewWorkflowStatus);
                return Ok(retValWithReviewInfo);
            }

            var retVal = _planDetailsService.GetPlanDetailsForRisk(quoteReferenceNumber, riskId);
            return Ok(retVal);
        }

        [HttpPost]
        [Route("risk/{riskId}/multi")]
        public IHttpActionResult UpdatePlans(int riskId, UpdatePlansRequest plansRequest)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var quoteReferenceNumber = _quoteSessionContext.QuoteSession.QuoteReference;

            var plansState = plansRequest.Requests.Select(planRequest => GetPlanState(riskId, planRequest, quoteReferenceNumber)).ToList();

            var planValidation = plansState.Select(p => new
            {
                PlanState = p,
                Validation = _activePlanValidationService.ValidateCurrentActivePlan(p)
            }).ToList();

            var hasErrors = false;
            foreach (var plan in planValidation)
            {
                if (!plan.Validation.HasErrors) continue;

                _errorsAndWarningsConverter.MapPlanValidationsToModelState(ModelState, plan.Validation.Errors, plan.PlanState.PlanCode);
                hasErrors = true;
            }

            //if partial update allowed, update plans with no errors, then throw if errors 
            //otherwise, throw if errors first
            if (plansRequest.AllowPartialUpdate)
            {
                foreach (var plan in planValidation.Where(p => !p.Validation.HasErrors && p.Validation.IsEligible))
                {
                    UpdatePlan(plan.PlanState, quoteReferenceNumber);
                }
                
                if (hasErrors)
                {
                    return new InvalidModelStateActionResult(ModelState);
                }
            }
            else
            {
                if (hasErrors)
                {
                    return new InvalidModelStateActionResult(ModelState);
                }

                foreach (var plan in planValidation)
                {
                    UpdatePlan(plan.PlanState, quoteReferenceNumber);
            		_planAutoUpdateService.UpdatePlansForRiskIdToConformWithPlanEligiblityRules(riskId);
                }
            }
            
            var retVal = _planDetailsService.GetPlanDetailsForRisk(quoteReferenceNumber, riskId);
            return Ok(retVal);
        }

        private PlanStateParam GetPlanState(int riskId, UpdatePlanRequest planRequest, string quoteReferenceNumber)
        {
            var userSelectedPlanState = _planStateParamFactory.CreatePlanStateParam(planRequest, quoteReferenceNumber, riskId);
            var autoUpdateResult = _planAutoUpdateService.UpdatePlanStateToConformToProductRules(userSelectedPlanState);
            var planState = autoUpdateResult.UpdatedPlanState;

            return planState;
        }

        [HttpPost, Route("risk/{riskId:int}/attach")]
        public IHttpActionResult AttachRider(int riskId, AttachRiderRequest attachRiderRequest)
        {
            throw new NotImplementedException("This feature is disabled");

            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var quoteReferenceNumber = _quoteSessionContext.QuoteSession.QuoteReference;
            _planRiderService.AttachRider(quoteReferenceNumber, riskId, attachRiderRequest.PlanToBecomeRiderCode, attachRiderRequest.PlanToHostRiderCode);

            var retVal = _planDetailsService.GetPlanDetailsForRisk(quoteReferenceNumber, riskId);
            return Ok(retVal);
        }

        [HttpPost, Route("risk/{riskId:int}/attach/multi")]
        public IHttpActionResult AttachRiderMulti(int riskId, AttachRiderRequestMulti requestMulti)
        {
            throw new NotImplementedException("This feature is disabled");

            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var quoteReferenceNumber = _quoteSessionContext.QuoteSession.QuoteReference;

            foreach (var request in requestMulti.Requests)
            {
                _planRiderService.AttachRider(quoteReferenceNumber, riskId, request.PlanToBecomeRiderCode, request.PlanToHostRiderCode);
            }
            
            var retVal = _planDetailsService.GetPlanDetailsForRisk(quoteReferenceNumber, riskId);
            return Ok(retVal);
        }

        [HttpPost, Route("risk/{riskId:int}/detach")]
        public IHttpActionResult DetachRider(int riskId, DetachRiderRequest detachRiderRequest)
        {
            throw new NotImplementedException("This feature is disabled");

            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var quoteReferenceNumber = _quoteSessionContext.QuoteSession.QuoteReference;
            _planRiderService.DetachRider(quoteReferenceNumber, riskId, detachRiderRequest.RiderCode);

            var retVal = _planDetailsService.GetPlanDetailsForRisk(quoteReferenceNumber, riskId);
            return Ok(retVal);
        }

        [HttpGet, Route("risk/{riskId:int}/validate")]
        public IHttpActionResult ValidateCovers(int riskId, UpdatePlanRequest planRequest)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var plans = _planDetailsService.GetPlanDetailsForRisk(QuoteReferenceNumber, riskId);
            var minimumSelectionCriteria = plans.Plans.Where(p => p.IsSelected);

            if (!minimumSelectionCriteria.Any() || minimumSelectionCriteria.Any(p => p.Covers.All(c => !c.IsSelected)))
            {
                ModelState.AddModelError("minimumSelectionCriteria", "Sorry, you have not added any insurance options to your quote. Please reconsider your selection and resubmit, or call 131 825 or Click-To-Chat to get in touch with one of our Life Insurance specialists.");
                return new InvalidModelStateActionResult(ModelState);
            }

            return Ok();
        }

        [HttpGet, Route("risk/{riskId:int}/proceed")]
        public IHttpActionResult ValidateCoversAndProceed(int riskId, UpdatePlanRequest planRequest)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var plans = _planDetailsService.GetPlanDetailsForRisk(QuoteReferenceNumber, riskId);
            var minimumSelectionCriteria = plans.Plans.Where(p => p.IsSelected);

            if (!minimumSelectionCriteria.Any() || minimumSelectionCriteria.Any(p => p.Covers.All(c => !c.IsSelected)))
            {
                ModelState.AddModelError("minimumSelectionCriteria", "Sorry, you have not added any insurance options to your quote. Please reconsider your selection and resubmit, or call 131 825 or Click-To-Chat to get in touch with one of our Life Insurance specialists.");
                return new InvalidModelStateActionResult(ModelState);
            }

            _riskUnderwritingService.SetRiskStatusToIncomplete(riskId);

            var redirectUrl = Url.Route("Default", new { Controller = "Qualification", Action = "Index" });
            return new RedirectActionResult(redirectUrl);
        }


        [HttpPost, Route("risk/{riskId:int}/use-calc-results")]
        public IHttpActionResult UseCalculatorResults(int riskId)
        {
            if (!IsRiskValidForApplicationSession(riskId))
            {
                return BadRequest();
            }

            var policyInitialisationMetaData = _policyInitialisationMetadataService.GetPolicyInitialisationMetadataForSession();
            if (policyInitialisationMetaData?.CalculatorResultsJson == null)
            {
                return NotFound();
            }

            policyInitialisationMetaData.SetResultsUsed();
            _policyInitialisationMetadataService.SetPolicyInitialisationMetadataForSession(policyInitialisationMetaData);
            
            return Ok();
        }


        private GetPlanResponse GetPlanDetailsForRisk(int riskId)
        {
            var quoteReferenceNumber = _quoteSessionContext.QuoteSession.QuoteReference;
            var planDetailsForRisk = _planDetailsService.GetPlanDetailsForRisk(quoteReferenceNumber, riskId);
            return planDetailsForRisk;
        }

                    
        private void UpdatePlan(PlanStateParam planState, string quoteReferenceNumber)
        {
            _updatePlanService.UpdateSelectedPlans(planState);
            _updatePlanService.UpdateActivePlanAndCalculatePremium(quoteReferenceNumber, planState).ToArray();
            //Force ToArray so enumeration code runs

        }

    }

}

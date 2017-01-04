using System.Web.Http;
using TAL.QuoteAndApply.SalesPortal.Web.Factories;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.ServiceLayer.Product;
using System.Linq;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.Web.Shared.Converters;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/policy/{quoteReferenceNumber}/risk/{riskId}/plan")]
    public class PlanController : ApiController
    {
        private readonly IAvailablePlanOptionsProvider _availablePlanOptionsProvider;
        private readonly IPlanModelConverter _modelConverter;
        private readonly IActivePlanValidationService _activePlanValidationService;
        private readonly IUpdatePlanService _updatePlanService;
		private readonly ISalesPortalConfiguration _salesPortalConfiguration;
        private readonly IPlanStateParamFactory _planStateParamFactory;
        private readonly IPlanUpdateResponseConverter _planUpdateResponseConverter;
        private readonly IPlanDetailsService _planDetailsService;
        private readonly IErrorsAndWarningsConverter _errorsAndWarningsConverter;
        private readonly IPolicyRiskPlanStatusProvider _policyRiskPlanStatusProvider;

        private readonly IPolicyPremiumSummaryProvider _policyPremiumSummaryProvider;
        private readonly IRiskPremiumSummaryViewModelConverter _riskPremiumSummaryViewModelConverter;
        private readonly IProductBrandProvider _productBrandProvider;

        public PlanController(IAvailablePlanOptionsProvider availablePlanOptionsProvider,
            IPlanModelConverter modelConverter,
            IUpdatePlanService updatePlanService, 
            IActivePlanValidationService activePlanValidationService,
            ISalesPortalConfiguration salesPortalConfiguration, 
            IPlanStateParamFactory planStateParamFactory,
            IPlanUpdateResponseConverter planUpdateResponseConverter, 
            IPlanDetailsService planDetailsService, 
            IErrorsAndWarningsConverter errorsAndWarningsConverter, 
            IPolicyRiskPlanStatusProvider policyRiskPlanStatusProvider, 
            IPolicyPremiumSummaryProvider policyPremiumSummaryProvider, 
            IRiskPremiumSummaryViewModelConverter riskPremiumSummaryViewModelConverter, IProductBrandProvider productBrandProvider)
        {
            _availablePlanOptionsProvider = availablePlanOptionsProvider;
            _modelConverter = modelConverter;
            _updatePlanService = updatePlanService;
            _activePlanValidationService = activePlanValidationService;
            _salesPortalConfiguration = salesPortalConfiguration;
            _planStateParamFactory = planStateParamFactory;
            _planUpdateResponseConverter = planUpdateResponseConverter;
            _planDetailsService = planDetailsService;
            _errorsAndWarningsConverter = errorsAndWarningsConverter;
            _policyRiskPlanStatusProvider = policyRiskPlanStatusProvider;
            _policyPremiumSummaryProvider = policyPremiumSummaryProvider;
            _riskPremiumSummaryViewModelConverter = riskPremiumSummaryViewModelConverter;
            _productBrandProvider = productBrandProvider;
        }

        [HttpPost, Route("availability")]
        public IHttpActionResult Availability(string quoteReferenceNumber, int riskId, SelectedPlanOptions selectedPlanOptions)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
            var inputModel = _modelConverter.GetSelectedPlanOptionsParam(riskId, brandKey, selectedPlanOptions);
            var planResult = _salesPortalConfiguration.ValidatePlansInRealTime 
                ? _availablePlanOptionsProvider.GetForPlan(inputModel)
                : _availablePlanOptionsProvider.GetForPlanAlwaysAvailable(inputModel);
            var outputModel = _modelConverter.GetAvailablePlanOptions(planResult);

            return Ok(outputModel);
        }

        [HttpGet, Route("getPlansAndCovers")]
        public IHttpActionResult GetPlansAndCovers(string quoteReferenceNumber, int riskId)
        {
            var returnObject = _planDetailsService.GetRiskPlanDetailsResponse(quoteReferenceNumber, riskId);
            return Ok(returnObject);
        }

        [HttpGet, Route("readyForValidation")]
        public IHttpActionResult VerifyForInForce(string quoteReferenceNumber, int riskId)
        {
            var returnObject = _planDetailsService.GetPlanDetailsForRisk(quoteReferenceNumber, riskId);
            return Ok(returnObject.Where(plan => plan.Selected).All(plan => plan.IsFilledIn));
        }
        
        [HttpPost, Route("edit")]
        public IHttpActionResult Edit(string quoteReferenceNumber, int riskId, PlanUpdateRequest updatePlanRequest)
        {
            var planConfigOptions = _planStateParamFactory.CreateActivePlanStateParam(updatePlanRequest, riskId);
            var editPlanResponse = _activePlanValidationService.ValidateCurrentActivePlan(planConfigOptions);

            if (editPlanResponse.HasErrors)
            {
                _errorsAndWarningsConverter.MapPlanValidationsToModelState(ModelState, editPlanResponse.Errors, updatePlanRequest.CurrentActivePlan.PlanCode);
                return new InvalidModelStateActionResult(ModelState);
            }

            _updatePlanService.UpdateSelectedPlans(planConfigOptions);
            var planPremiumResult = _updatePlanService.UpdateActivePlanAndCalculatePremium(quoteReferenceNumber, planConfigOptions).ToArray();

            //run full validation for all Plans
            var plansStatus = _policyRiskPlanStatusProvider.GetFor(quoteReferenceNumber, riskId);

            //get the premium summary object
            var policyPremiumSummary = _policyPremiumSummaryProvider.GetFor(quoteReferenceNumber);
            var riskPremium = _riskPremiumSummaryViewModelConverter.CreateFrom(riskId, policyPremiumSummary);

            //map the validation onto the plans
            var returnObject = _planUpdateResponseConverter.From(updatePlanRequest, editPlanResponse, planPremiumResult, plansStatus, riskPremium);

            return Ok(returnObject);
        }
    }
}
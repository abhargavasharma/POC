using System;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanRiderService
    {
        void AttachRider(string quoteReferenceNumber, int riskId, string planToBecomeRiderCode, string planToAttachRiderToCode);
        void DetachRider(string quoteReferenceNumber, int riskId, string riderToDetachCode);
    }

    public class PlanRiderService : IPlanRiderService
    {
        private readonly IPlanOverviewResultProvider _planOverviewResultProvider;
        private readonly IPlanResponseProvider _planResponseProvider;
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly IPlanResponseToStateParamConverter _planResponseToStateParamConverter;
        private readonly IRiderAttachmentService _riderAttachmentService;
        private readonly IUpdatePlanService _updatePlanService;
        private readonly IProductBrandProvider _productBrandProvider;

        public PlanRiderService(IPlanOverviewResultProvider planOverviewResultProvider,
            IPlanResponseProvider planResponseProvider, IProductDefinitionProvider productDefinitionProvider,
            IPlanResponseToStateParamConverter planResponseToStateParamConverter,
            IRiderAttachmentService riderAttachmentService, IUpdatePlanService updatePlanService, IProductBrandProvider productBrandProvider)
        {
            _planOverviewResultProvider = planOverviewResultProvider;
            _planResponseProvider = planResponseProvider;
            _productDefinitionProvider = productDefinitionProvider;
            _planResponseToStateParamConverter = planResponseToStateParamConverter;
            _riderAttachmentService = riderAttachmentService;
            _updatePlanService = updatePlanService;
            _productBrandProvider = productBrandProvider;
        }

        private PlanStateParam[] GetPlanStates(string quoteReferenceNumber, int riskId, ProductDetailsResult productDefinition)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
            var policyPlans = _planOverviewResultProvider.GetFor(riskId).ToArray();
            var poicyId = policyPlans.FirstOrDefault()?.PolicyId ?? 0;
            return
                _planResponseProvider.MapStoredPlansToProduct(quoteReferenceNumber,
                    productDefinition.Plans, policyPlans)
                    .Select(pr => _planResponseToStateParamConverter.CreateFrom(pr,
                        PlanStateParam.BuildBasicPlanStateParam(pr.Code, brandKey, pr.Selected, poicyId, riskId, null,
                            pr.CoverAmount, null, PremiumType.Unknown, pr.PlanId, null, null, OccupationDefinition.Unknown)))
                    .ToArray();
        }

        public void DetachRider(string quoteReferenceNumber, int riskId, string riderToDetachCode)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
            var productDefinition = _productDefinitionProvider.GetProductDefinition(brandKey);
            var planResponses = GetPlanStates(quoteReferenceNumber, riskId, productDefinition);

            var riderDefinition = productDefinition.Plans.Where(p => p.Riders?.Any() ?? false)
                .SelectMany(p => p.Riders)
                .First(r => r.Code.Equals(riderToDetachCode, StringComparison.OrdinalIgnoreCase));

            var riderParentPlanState = planResponses.Where(p => p.Riders?.Any() ?? false)
                .First(pr =>
                pr.Riders.Any(r => r.PlanCode.Equals(riderToDetachCode, StringComparison.OrdinalIgnoreCase)));

            var riderPlanState = planResponses.Where(p => p.Riders?.Any() ?? false)
                .SelectMany(p => p.Riders)
                .FirstOrDefault(pr => 
                pr.PlanCode.Equals(riderToDetachCode, StringComparison.OrdinalIgnoreCase));

            var relatedPlanState = planResponses.FirstOrDefault(
                pr => pr.PlanCode.Equals(riderDefinition.RelatedPlanCode, StringComparison.OrdinalIgnoreCase));

            _riderAttachmentService.DetachRider(riderPlanState, relatedPlanState);

            _updatePlanService.UpdateActivePlanAndCalculatePremium(quoteReferenceNumber, riderParentPlanState).ToArray();
            _updatePlanService.UpdateActivePlanAndCalculatePremium(quoteReferenceNumber, relatedPlanState).ToArray();
        }

        public void AttachRider(string quoteReferenceNumber, int riskId, string planToBecomeRiderCode, string planToAttachRiderToCode)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
            var productDefinition = _productDefinitionProvider.GetProductDefinition(brandKey);
            var planResponses = GetPlanStates(quoteReferenceNumber, riskId, productDefinition);

            var planToBecomeRider = planResponses.FirstOrDefault(
                pr => pr.PlanCode.Equals(planToBecomeRiderCode, StringComparison.OrdinalIgnoreCase));

            var planToAttachRiderTo = planResponses.FirstOrDefault(
                pr => pr.PlanCode.Equals(planToAttachRiderToCode, StringComparison.OrdinalIgnoreCase));

            _riderAttachmentService.AttachRider(planToBecomeRider, planToAttachRiderTo);

            _updatePlanService.UpdateActivePlanAndCalculatePremium(quoteReferenceNumber, planToBecomeRider).ToArray();
            _updatePlanService.UpdateActivePlanAndCalculatePremium(quoteReferenceNumber, planToAttachRiderTo).ToArray();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IRiderAttachmentService
    {
        void AttachRider(PlanStateParam planToBecomeRider, PlanStateParam planToAttachTo);
        void DetachRider(PlanStateParam riderPlan, PlanStateParam relatedPlan);
    }

    public class RiderAttachmentService : IRiderAttachmentService
    {
        private readonly IProductDefinitionService _productDefinitionService;

        public RiderAttachmentService(IProductDefinitionService productDefinitionService)
        {
            _productDefinitionService = productDefinitionService;
        }

        public void AttachRider(PlanStateParam planToBecomeRider, PlanStateParam planToAttachTo)
        {
            var productDefinition = _productDefinitionService.GetProductDefinition();
            var planToAttchToDefintion = productDefinition.Plans.First(
                p => p.Code.Equals(planToAttachTo.PlanCode, StringComparison.OrdinalIgnoreCase));
            var planToBecomeRiderDefinition = productDefinition.Plans.First(
                p => p.Code.Equals(planToBecomeRider.PlanCode, StringComparison.OrdinalIgnoreCase));

            var rider = planToAttchToDefintion?.Riders.FirstOrDefault(
                r => r.RelatedPlanCode.Equals(planToBecomeRiderDefinition.Code));

            if (rider == null)
            {
                throw new ApplicationException($"Could not attach {planToBecomeRider.PlanCode} to {planToAttachTo.PlanCode}. No related rider exists.");
            }

            UpdateRiderState(rider, planToAttachTo, planToBecomeRiderDefinition, planToBecomeRider);
        }

        public void DetachRider(PlanStateParam riderPlan, PlanStateParam relatedPlan)
        {
            var productDefinition = _productDefinitionService.GetProductDefinition();
            var riderDefinition = productDefinition.Plans.Where(p => p.Riders?.Any() ?? false)
                .SelectMany(p => p.Riders)
                .First(r => r.Code.Equals(riderPlan.PlanCode, StringComparison.OrdinalIgnoreCase));

            var relatedPlanDefinition = productDefinition.Plans.First(
                p => p.Code.Equals(relatedPlan.PlanCode, StringComparison.OrdinalIgnoreCase));

            UpdateRelatedPlanState(riderDefinition, riderPlan, relatedPlanDefinition, relatedPlan);
        }

        public void UpdateRiderState(PlanDefinition rider, PlanStateParam planToAttachTo,
            PlanDefinition planToBecomeRiderDefinition, PlanStateParam planToBecomeRider)
        {
            var coverCodeMap = rider.Covers.ToDictionary(k => k.CoverFor, v => v.Code);

            var riderState =
                planToAttachTo.Riders.First(r => r.PlanCode.Equals(rider.Code, StringComparison.OrdinalIgnoreCase));
            riderState.Selected = true;
            riderState.UpdateSelectedCovers(planToBecomeRider.SelectedCoverCodes.Select(c => coverCodeMap[c]));
            riderState.CoverAmount = planToBecomeRider.CoverAmount;

            planToBecomeRider.Selected = false;
        }

        public void UpdateRelatedPlanState(PlanDefinition rider, PlanStateParam riderState,
            PlanDefinition relatedPlanDefinitions, PlanStateParam relatedPlan)
        {
            var coverCodeMap = rider.Covers.ToDictionary(v => v.Code, k => k.CoverFor);
            relatedPlan.UpdateSelectedCovers(riderState.SelectedCoverCodes.Select(c => coverCodeMap[c]));
            relatedPlan.Selected = false;
            relatedPlan.CoverAmount = riderState.CoverAmount;
            riderState.Selected = false;
        }
    }
}

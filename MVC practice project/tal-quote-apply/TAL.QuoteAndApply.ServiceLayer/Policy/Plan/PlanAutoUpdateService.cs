using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanAutoUpdateService
    {
        UpdatedPlanStateContainer UpdatePlanStateToConformToProductRules(PlanStateParam planState);
        void UpdatePlansToConformWithPlanEligiblityRules(IRisk risk);
        void UpdatePlansForRiskIdToConformWithPlanEligiblityRules(int riskId);
    }

    public class PlanAutoUpdateService : IPlanAutoUpdateService
    {
        private readonly IAvailablePlanOptionsProvider _availablePlanOptionsProvider;
        private readonly IPlanService _planService;
        private readonly IPlanEligibilityService _planEligibilityService;
        private readonly ICoverService _coverService;
        private readonly ICoverEligibilityService _coverEligibilityService;
        private readonly IRiskService _riskService;
        private readonly IPlanCoverAmountService _planCoverAmountService;
        private readonly IUpdateMarketingStatusService _updateMarketingStatusService;

        public PlanAutoUpdateService(IAvailablePlanOptionsProvider availablePlanOptionsProvider,
            IPlanService planService, IPlanEligibilityService planEligibilityService, ICoverService coverService,
            ICoverEligibilityService coverEligibilityService, IRiskService riskService, 
            IPlanCoverAmountService planCoverAmountService, 
            IUpdateMarketingStatusService updateMarketingStatusService)
        {
            _availablePlanOptionsProvider = availablePlanOptionsProvider;
            _planService = planService;
            _planEligibilityService = planEligibilityService;
            _coverService = coverService;
            _coverEligibilityService = coverEligibilityService;
            _riskService = riskService;
            _planCoverAmountService = planCoverAmountService;
            _updateMarketingStatusService = updateMarketingStatusService;
        }

        public UpdatedPlanStateContainer UpdatePlanStateToConformToProductRules(PlanStateParam planState)
        {
            var newPlanState = planState.Clone();

            var risk = _riskService.GetRisk(planState.RiskId);

            var availabilityResult = _availablePlanOptionsProvider.GetForPlan(newPlanState.ToAvailabilityPlanStateParam(risk.OccupationClass));

            foreach (var planIdentityInfo in newPlanState.AllPlans)
            {
                planIdentityInfo.Selected = planIdentityInfo.Selected &&
                    availabilityResult.AvailablePlans.Any(p => planIdentityInfo.PlanCode.Equals(p, StringComparison.OrdinalIgnoreCase));
            }

            var coversToRemove = newPlanState.SelectedCoverCodes.Except(availabilityResult.AvailableCovers);
            newPlanState.UpdateSelectedCovers(newPlanState.SelectedCoverCodes.Except(coversToRemove));

            foreach (var planOption in newPlanState.PlanOptions)
            {
                if (planOption.Selected.HasValue)
                {
                    planOption.Selected = planOption.Selected.Value &&
                        availabilityResult.AvailableOptions.Any(o => planOption.Code.Equals(o, StringComparison.OrdinalIgnoreCase));
                }
            }

            foreach (var rider in newPlanState.Riders)
            {
                var availableRider = availabilityResult.AvailableRiders.FirstOrDefault(
                        r => r.RiderCode.Equals(rider.PlanCode, StringComparison.OrdinalIgnoreCase));
                rider.Selected = rider.Selected &&
                    availabilityResult.AvailableRiders.Any(p => rider.PlanCode.Equals(p.RiderCode, StringComparison.OrdinalIgnoreCase));

                var riderCoversToRemove = rider.SelectedCoverCodes.Except(availableRider?.AvailableCovers ?? new string[0]);
                rider.UpdateSelectedCovers(rider.SelectedCoverCodes.Except(riderCoversToRemove));

                foreach (var planOption in rider.PlanOptions)
                {
                    if (planOption.Selected.HasValue)
                    {
                        planOption.Selected = planOption.Selected.Value &&
                                              (availableRider?.AvailableOptions.Any(
                                                  o => planOption.Code.Equals(o, StringComparison.OrdinalIgnoreCase)) ??
                                               false);
                    }
                }
            }

            return new UpdatedPlanStateContainer(newPlanState, availabilityResult.UnavailableFeatures);

        }

        public void UpdatePlansToConformWithPlanEligiblityRules(IRisk risk)
        {
            var plans = _planService.GetPlansForRisk(risk.Id);
            
            foreach (var plan in plans)
            {
                var planCovers = _coverService.GetCoversForPlan(plan.Id);
                var planCoverEligibiltyResults = _coverEligibilityService.GetCoverEligibilityResults(planCovers);

                var planChanged = false;
                
                if (!_planEligibilityService.IsRiskEligibleForPlan(risk, plan, planCoverEligibiltyResults).IsAvailable)
                {
                    plan.Selected = false;
                    planChanged = true;
                }

                var planCoverAmounToMax = _planCoverAmountService.ChangePlanCoverAmountToMinOrMaxIfApplicable(risk, plan, plans);
                
                if (planChanged || planCoverAmounToMax.CoverAmountChanged)
                {
                    _planService.UpdatePlan(plan);
                }
                
                foreach (var cover in planCovers)
                {
                    var coverEligibility = planCoverEligibiltyResults.Single(e => e.CoverCode == cover.Code);
                    if (cover.Selected && !coverEligibility.EligibleForCover)
                    {
                        cover.Selected = false;
                        _coverService.UpdateCover(cover);
                    }
                }
            }
            _updateMarketingStatusService.UpdateMarketingStatusForRisk(risk.Id);
        }

        public void UpdatePlansForRiskIdToConformWithPlanEligiblityRules(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            UpdatePlansToConformWithPlanEligiblityRules(risk);
        }
    }
}

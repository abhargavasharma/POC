using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPlanStateParamFactory
    {
        PlanStateParam CreateActivePlanStateParam(PlanUpdateRequest updatePlanRequest, int riskId);
    }

    public class PlanStateParamFactory : IPlanStateParamFactory
    {
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IPlanDetailsService _planDetailsService;

        public PlanStateParamFactory(IPolicyOverviewProvider policyOverviewProvider, IPlanDetailsService planDetailsService)
        {
            _policyOverviewProvider = policyOverviewProvider;
            _planDetailsService = planDetailsService;
        }

        public PlanStateParam CreateActivePlanStateParam(PlanUpdateRequest updatePlanRequest, int riskId)
        {
            var policyOverview = _policyOverviewProvider.GetFor(updatePlanRequest.QuoteReferenceNumber);
            var riskOverview = policyOverview.Risks.First(r => r.RiskId == riskId);

            var plans = _planDetailsService.GetPlanDetailsForRisk(policyOverview.QuoteReferenceNumber, riskOverview.RiskId);

            return CreateActivePlanStateParam(updatePlanRequest, riskOverview, plans, policyOverview.PolicyId, policyOverview.Brand);
        }

        private PlanStateParam CreateNonActivePlanStateParam(PlanDetailResponse planDetailResponse, int riskId, string quoteReference, PlanUpdateRequest updatePlanRequest, IEnumerable<PlanDetailResponse> allPlanDetailResponses)
        {
            var policyOverview = _policyOverviewProvider.GetFor(quoteReference);
            var riskOverview = policyOverview.Risks.First(r => r.RiskId == riskId);

            var plans = _planDetailsService.GetPlanDetailsForRisk(policyOverview.QuoteReferenceNumber, riskOverview.RiskId);

            return CreateNonActivePlanStateParam(planDetailResponse, riskOverview, plans, policyOverview.PolicyId, updatePlanRequest, allPlanDetailResponses, policyOverview.Brand);
        }

        private PlanStateParam CreateActivePlanStateParam(PlanUpdateRequest updatePlanRequest,
            RiskOverviewResult risk, IEnumerable<PlanDetailResponse> plans, int policyId, string brandKey)
        {
            var planInfos = plans.Select(p => new PlanIdentityInfo
            {
                PlanCode = p.Code,
                PlanId = p.PlanId,
                Selected =
                    updatePlanRequest.SelectedPlanCodes.Any(
                        spc => p.Code.Equals(spc, StringComparison.OrdinalIgnoreCase))
            });

            return PlanStateParam.BuildPlanStateParam(
                coverAmount: updatePlanRequest.CurrentActivePlan.CoverAmount,
                age: risk.Age,
                income: risk.AnnualIncome,
                planId: updatePlanRequest.CurrentActivePlan.PlanId,
                selectedCoverCodes: updatePlanRequest.CurrentActivePlan.SelectedCoverCodes,
                riders: updatePlanRequest.CurrentActivePlan.SelectedRiders.Select(
                        r => BuildActiveRider(r, updatePlanRequest.CurrentActivePlan, risk, policyId, brandKey)),
                planOptions: updatePlanRequest.CurrentActivePlan.SelectedOptionCodes.Select(BuildOptionParam).ToList(),
                allPlans: planInfos,
                policyId: policyId,
                riskId: risk.RiskId,
                planCode: updatePlanRequest.CurrentActivePlan.PlanCode,
                linkedToCpi: updatePlanRequest.CurrentActivePlan.LinkedToCpi,
                selected: updatePlanRequest.SelectedPlanCodes.Contains(updatePlanRequest.CurrentActivePlan.PlanCode),
                premiumHoliday: updatePlanRequest.CurrentActivePlan.PremiumHoliday,
                premiumType: updatePlanRequest.CurrentActivePlan.PremiumType.MapToPremiumType(),
                waitingPeriod: updatePlanRequest.CurrentActivePlan.WaitingPeriod,
                benefitPeriod: updatePlanRequest.CurrentActivePlan.BenefitPeriod,
                occupationDefinition: updatePlanRequest.CurrentActivePlan.OccupationDefinition.MapToOccupationDefinition(),
                brandKey: brandKey
            );
        }

        private PlanStateParam CreateNonActivePlanStateParam(PlanDetailResponse planDetailResponse,
            RiskOverviewResult risk, IEnumerable<PlanDetailResponse> plans, int policyId, //Below two parameters added for other plan validation to be done.
            PlanUpdateRequest updatePlanRequest, IEnumerable<PlanDetailResponse> allPlanDetailResponses,
            string brandKey)
        {
            var planInfos = plans.Select(p => new PlanIdentityInfo
            {
                PlanCode = p.Code,
                PlanId = p.PlanId,
                Selected =
                    updatePlanRequest.SelectedPlanCodes.Any(
                        spc => p.Code.Equals(spc, StringComparison.OrdinalIgnoreCase))
            });

            var allRiders = allPlanDetailResponses.SelectMany(x => x.Riders).Where(z => z.Selected).Select(y => BuildNonActivePlanRider(y, planDetailResponse, risk, policyId, brandKey));
            
            return PlanStateParam.BuildPlanStateParam(
                coverAmount: planDetailResponse.CoverAmount,
                age: risk.Age,
                income: risk.AnnualIncome,
                planId: planDetailResponse.PlanId,
                selectedCoverCodes: planDetailResponse.Covers.Where(x => x.Selected).Select(y => y.Code),
                riders: allRiders,
                planOptions: planDetailResponse.Options.Select(BuildOptionParam).ToList(),
                allPlans: planInfos,
                policyId: policyId,
                riskId: risk.RiskId,
                planCode: planDetailResponse.Code,
                linkedToCpi: planDetailResponse.LinkedToCpi,
                selected: updatePlanRequest.SelectedPlanCodes.Contains(planDetailResponse.Code),
                premiumHoliday: planDetailResponse.PremiumHoliday,
                premiumType: planDetailResponse.PremiumType.MapToPremiumType(),
                waitingPeriod: planDetailResponse.WaitingPeriod,
                benefitPeriod: planDetailResponse.BenefitPeriod,
                occupationDefinition: planDetailResponse.OccupationDefinition,
                brandKey: brandKey
                );
            
        }

        private PlanStateParam BuildNonActivePlanRider(PlanDetailResponse rider, PlanDetailResponse parentPlan, RiskOverviewResult risk, int policyId, string brandKey)
        {
            var riderOptions = rider.Options.Select(BuildOptionParam);
            var parentPlanOptions = parentPlan.Options.Select(BuildOptionParam);

            return PlanStateParam.BuildRiderPlanStateParam(rider.Code,
                brandKey,
                rider.Selected,
                policyId,
                risk.RiskId,
                parentPlan.LinkedToCpi,
                rider.CoverAmount,
                parentPlan.PremiumHoliday,
                parentPlan.PremiumType.MapToPremiumType(),
                rider.PlanId,
                risk.Age,
                risk.AnnualIncome,
                riderOptions.Concat(parentPlanOptions), 
                new string[0], //rider.Covers.Where(x => x.Selected).Select(y => y.Code) - Because of no dependencies between other plans and rider covers,
                parentPlan.CoverAmount,
                rider.OccupationDefinition
                );
        }

        private PlanStateParam BuildActiveRider(PlanConfigurationRequest rider, PlanConfigurationRequest parentPlan, RiskOverviewResult risk, int policyId, string brandKey)
        {
            var riderOptions = rider.SelectedOptionCodes.Select(BuildOptionParam);
            var parentPlanOptions = parentPlan.SelectedOptionCodes.Select(BuildOptionParam);

            return PlanStateParam.BuildRiderPlanStateParam(rider.PlanCode,
                brandKey,
                rider.Selected,
                policyId,
                risk.RiskId,
                parentPlan.LinkedToCpi,
                rider.CoverAmount,
                parentPlan.PremiumHoliday,
                parentPlan.PremiumType.MapToPremiumType(),
                rider.PlanId,
                risk.Age,
                risk.AnnualIncome,
                riderOptions.Concat(parentPlanOptions),
                rider.SelectedCoverCodes,
                parentPlan.CoverAmount,
                rider.OccupationDefinition.MapToOccupationDefinition()
                );
        }

        private OptionsParam BuildOptionParam(OptionConfigurationRequest option)
        {
            return new OptionsParam(option.Code, option.Selected);
        }

        private OptionsParam BuildOptionParam(PlanDetailOptionResponse option)
        {
            return new OptionsParam(option.Code, option.Selected);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.Customer.Web.Services.Converters
{
    public interface IPlanStateParamFactory
    {
        PlanStateParam CreatePlanStateParam(UpdatePlanRequest updatePlanRequest, string quoteReferenceNumber, int riskId);
    }

    public class PlanStateParamFactory : IPlanStateParamFactory
    {
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IPlanDetailsService _planDetailsService;
        private readonly IPlanVariableResponseConverter _planVariableResponseConverter;

        public PlanStateParamFactory(IPolicyOverviewProvider policyOverviewProvider,
            IPlanDetailsService planDetailsService, IPlanVariableResponseConverter planVariableResponseConverter)
        {
            _policyOverviewProvider = policyOverviewProvider;
            _planDetailsService = planDetailsService;
            _planVariableResponseConverter = planVariableResponseConverter;
        }
        
        public PlanStateParam CreatePlanStateParam(UpdatePlanRequest updatePlanRequest, string quoteReferenceNumber, int riskId)
        {
            var policyOverview = _policyOverviewProvider.GetFor(quoteReferenceNumber);
            var riskOverview = policyOverview.Risks.First(r => r.RiskId == riskId);

            var plans = _planDetailsService.GetPlanDetailsForRisk(policyOverview.QuoteReferenceNumber, riskOverview.RiskId);

            var updatePlanVariables = updatePlanRequest.Variables?.Select(
                v => new VariableResponse {Code = v.Code, SelectedValue = v.SelectedValue}).ToList() ?? new List<VariableResponse>();

            return CreatePlanStateParam(updatePlanRequest, updatePlanVariables, riskOverview, plans.Plans, policyOverview.PolicyId, policyOverview.Brand);
        }
        
        private PlanStateParam CreatePlanStateParam(UpdatePlanRequest updatePlanRequest, List<VariableResponse> selectedPlanVariables,
            RiskOverviewResult risk, IEnumerable<PlanSelectionResponse> plans, int policyId, string brandKey)
        {
            var planInfos = plans.Select(p => new PlanIdentityInfo(p.PlanId, p.PlanCode, 
                    updatePlanRequest.SelectedPlans.Any(spc => p.PlanCode.Equals(spc, StringComparison.OrdinalIgnoreCase)))
                );

            return PlanStateParam.BuildPlanStateParam(
                coverAmount: updatePlanRequest.SelectedCoverAmount,
                age: risk.Age,
                income: risk.AnnualIncome,
                planId: updatePlanRequest.PlanId,
                selectedCoverCodes: updatePlanRequest.SelectedCovers,
                riders: updatePlanRequest.Riders.Select(
                    r => BuildRider(r, updatePlanRequest, selectedPlanVariables, risk, policyId, brandKey)),
                planOptions: updatePlanRequest.Options.Select(BuildOptionParam).ToList(),
                allPlans: planInfos,
                policyId: policyId,
                riskId: risk.RiskId,
                planCode: updatePlanRequest.PlanCode,
                linkedToCpi: _planVariableResponseConverter.CpiFrom(selectedPlanVariables),
                selected: updatePlanRequest.SelectedPlans.Contains(updatePlanRequest.PlanCode),
                premiumHoliday: updatePlanRequest.PremiumHoliday,
                premiumType: updatePlanRequest.PremiumType.MapToPremiumType(),
                waitingPeriod: _planVariableResponseConverter.WaitingPeriodFrom(selectedPlanVariables),
                benefitPeriod: _planVariableResponseConverter.BenefitPeriodFrom(selectedPlanVariables),
				occupationDefinition: _planVariableResponseConverter.OccupationDefinitionFrom(selectedPlanVariables).MapToOccupationDefinition(),
                brandKey: brandKey
                );
        }

        private PlanStateParam BuildRider(PlanRiderRequest rider, UpdatePlanRequest parentPlan,
            List<VariableResponse> parentPlanVariables, RiskOverviewResult risk, int policyId, string brandKey)
        {
            var riderOptions = rider.Options.Select(BuildOptionParam);
            var parentPlanOptions = parentPlan.Options.Select(BuildOptionParam);

            return PlanStateParam.BuildRiderPlanStateParam(rider.PlanCode,
                brandKey,
                rider.IsSelected,
                policyId,
                risk.RiskId,
                _planVariableResponseConverter.CpiFrom(parentPlanVariables),
                rider.SelectedCoverAmount,
                parentPlan.PremiumHoliday,
                parentPlan.PremiumType.MapToPremiumType(),
                rider.PlanId,
                risk.Age,
                risk.AnnualIncome,
                riderOptions.Concat(parentPlanOptions),
                rider.SelectedCovers,
                parentPlan.SelectedCoverAmount,
                rider.OccupationDefinition.MapToOccupationDefinition()
                );
        }

        public OptionsParam BuildOptionParam(OptionConfigurationRequest option)
        {
            return new OptionsParam(option.Code, option.IsSelected);
        }

    }
}
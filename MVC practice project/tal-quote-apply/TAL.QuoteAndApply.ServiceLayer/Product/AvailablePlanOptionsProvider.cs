using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public interface IAvailablePlanOptionsProvider
    {
        AvailablePlanOptionsAndConfigResult GetForPlan(AvailabilityPlanStateParam selectedPlanOptions);
        AvailablePlanOptionsAndConfigResult GetForAllPlans(AvailabilityPlanStateParam selectedPlanOptions);
        AvailablePlanOptionsAndConfigResult GetForPlanAlwaysAvailable(AvailabilityPlanStateParam selectedPlanOptions);
    }
    
    public class AvailablePlanOptionsProvider : IAvailablePlanOptionsProvider
    {
        private readonly IPlanOptionAvailabilityProvider _planOptionAvailabilityProvider;
        private readonly ICoverAvailabilityProvider _coverAvailabilityProvider;
        private readonly IPlanAvailabilityProvider _policyPlanAvailabilityProvider;
        private readonly IPlanRiderAvailabilityProvider _planRiderAvailabilityProvider;
        private readonly IPlanEligibilityProvider _planEligibilityProvider;
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly IVariableAvailabilityProvider _variableAvailabilityProvider;
        private readonly IRiskOccupationService _riskOccupationService;

        public AvailablePlanOptionsProvider(IPlanOptionAvailabilityProvider planOptionAvailabilityProvider,
            ICoverAvailabilityProvider coverAvailabilityProvider,
            IPlanAvailabilityProvider policyPlanAvailabilityProvider,
            IPlanRiderAvailabilityProvider planRiderAvailabilityProvider,
            IPlanEligibilityProvider planEligibilityProvider, IProductDefinitionProvider productDefinitionProvider, IVariableAvailabilityProvider variableAvailabilityProvider, IRiskOccupationService riskOccupationService)
        {
            _planOptionAvailabilityProvider = planOptionAvailabilityProvider;
            _coverAvailabilityProvider = coverAvailabilityProvider;
            _policyPlanAvailabilityProvider = policyPlanAvailabilityProvider;
            _planRiderAvailabilityProvider = planRiderAvailabilityProvider;
            _planEligibilityProvider = planEligibilityProvider;
            _productDefinitionProvider = productDefinitionProvider;
            _variableAvailabilityProvider = variableAvailabilityProvider;
            _riskOccupationService = riskOccupationService;
        }

        public AvailablePlanOptionsAndConfigResult GetForAllPlans(AvailabilityPlanStateParam selectedPlanOptions)
        {
            var originalPlan = selectedPlanOptions.PlanCode;
            var results = new List<AvailablePlanOptionsAndConfigResult>();
            var allPlanCodes = _productDefinitionProvider.GetProductDefinition(selectedPlanOptions.BrandKey).Plans.Select(p => p.Code);
            foreach (var selectedPlanCode in allPlanCodes)
            {
                selectedPlanOptions.PlanCode = selectedPlanCode;
                results.Add(GetForPlan(selectedPlanOptions));
            }
            selectedPlanOptions.PlanCode = originalPlan;
            return new AvailablePlanOptionsAndConfigResult(
                originalPlan, 
                results.SelectMany(r => r.AvailableOptions).Distinct(),
                results.SelectMany(r => r.AvailableCovers).Distinct(),
                results.SelectMany(r => r.AvailablePlans).Distinct(),
                results.SelectMany(r => r.AvailableRiders).DistinctBy(d => d.RiderCode),
                results.SelectMany(r => r.UnavailableFeatures).DistinctBy(d => d.Code),
                results.SelectMany(r => r.VariableAvailability).DistinctBy(v => v.Code));
        }

        public AvailablePlanOptionsAndConfigResult GetForPlanAlwaysAvailable(AvailabilityPlanStateParam selectedPlanOptions)
        {
            var availablePlanOptions = _planOptionAvailabilityProvider.GetAllOptions(selectedPlanOptions).ToArray();
            var availableCovers = _coverAvailabilityProvider.GetAllCovers(selectedPlanOptions).ToArray();
            var availablePlans = _policyPlanAvailabilityProvider.GetAllPlans(selectedPlanOptions).ToArray();
            var availableRiders = _planRiderAvailabilityProvider.GetAllRiders(selectedPlanOptions).ToArray();

            var riderResults = availableRiders.Select(rider =>
            {
                return new AvailableRiderOptionsAndConfigResult()
                {
                    RiderCode = rider.Code,
                    AvailableCovers = _planRiderAvailabilityProvider.GetAllRiderCovers(rider.Code,
                    selectedPlanOptions).Select(af => af.Code),
                    AvailableOptions = _planRiderAvailabilityProvider.GetAllRiderOptions(rider.Code,
                    selectedPlanOptions).Select(af => af.Code),
                    UnavailableFeatures = new AvailableFeature[0]
                };
            });

            return new AvailablePlanOptionsAndConfigResult(selectedPlanOptions.PlanCode,
                availablePlanOptions.Where(po => po.IsAvailable).Select(po => po.Code),
                availableCovers.Where(po => po.IsAvailable).Select(po => po.Code),
                availablePlans.Where(po => po.IsAvailable).Select(po => po.Code),
                riderResults, new AvailableFeature[0], new AvailableFeature[0]);
        }

        public AvailablePlanOptionsAndConfigResult GetForPlan(AvailabilityPlanStateParam selectedPlanOptions)
        {
            var occupation = _riskOccupationService.GetForRiskId(selectedPlanOptions.RiskId);
            selectedPlanOptions.OccupationClass = occupation.OccupationClass;

            var eligibilityResults = _planEligibilityProvider.GetPlanEligibilitiesFor(selectedPlanOptions.RiskId);

            var availablePlanOptions = _planOptionAvailabilityProvider.GetAvailableOptions(selectedPlanOptions).ToArray();

            var allCoverEligibilityResults = eligibilityResults.SelectMany(p => p.CoverEligibilityResults).ToList();
            var availableCovers =
                _coverAvailabilityProvider.ApplyEligibility(
                    _coverAvailabilityProvider.GetAvailableCovers(selectedPlanOptions), allCoverEligibilityResults)
                    .ToArray();

            var availablePlans =
                _policyPlanAvailabilityProvider.ApplyEligibility(
                    _policyPlanAvailabilityProvider.GetAvailablePlans(selectedPlanOptions),
                    eligibilityResults)
                    .ToArray();

            var availableRiders = _planRiderAvailabilityProvider.GetAvailableRiders(selectedPlanOptions).ToArray();

            var riderResults = availableRiders.Select(rider =>
            {
                var availableRiderCovers = _planRiderAvailabilityProvider.GetAvailableRiderCovers(rider.Code,
                    selectedPlanOptions).ToArray();
                var availableRiderOptions = _planRiderAvailabilityProvider.GetAllRiderOptions(rider.Code,
                    selectedPlanOptions).ToArray();
                return new AvailableRiderOptionsAndConfigResult()
                {
                    RiderCode = rider.Code,
                    AvailableCovers = availableRiderCovers.Where(af => af.IsAvailable).Select(af => af.Code),
                    AvailableOptions = availableRiderOptions.Where(af => af.IsAvailable).Select(af => af.Code),
                    UnavailableFeatures = availableRiderCovers.Where(af => !af.IsAvailable).Concat(availableRiderOptions.Where(af => !af.IsAvailable))
                };
            });

            var availableVariables = _variableAvailabilityProvider.GetVariableAvailability(selectedPlanOptions).ToArray();

            var unavailableItems = availablePlanOptions.Where(po => !po.IsAvailable)
                .Concat(availableCovers.Where(po => !po.IsAvailable))
                .Concat(availablePlans.Where(po => !po.IsAvailable))
                .Concat(availableRiders.Where(po => !po.IsAvailable)).ToList();

            unavailableItems.AddRange(riderResults.SelectMany(x => x.UnavailableFeatures));

            return new AvailablePlanOptionsAndConfigResult(selectedPlanOptions.PlanCode,
                availablePlanOptions.Where(po => po.IsAvailable).Select(po => po.Code),
                availableCovers.Where(po => po.IsAvailable).Select(po => po.Code), 
                availablePlans.Where(po => po.IsAvailable).Select(po => po.Code),
                riderResults.ToArray(),
                unavailableItems, availableVariables);
        }
    }
}

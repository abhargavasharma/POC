using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Models.Mappers;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Rules
{
    public interface IPlanAvailabilityProvider
    {
        IEnumerable<AvailableFeature> GetAvailablePlans(AvailabilityPlanStateParam planOptionsParam);
        IEnumerable<AvailableFeature> GetAllPlans(AvailabilityPlanStateParam planOptionsParam);

        IEnumerable<AvailableFeature> ApplyEligibility(IEnumerable<AvailableFeature> availableFeatures,
            IEnumerable<PlanEligibilityResult> eligibilityResults);
    }

    public class PlanAvailabilityProvider : BaseAvailabilityProvider, IPlanAvailabilityProvider
    {

        public IEnumerable<AvailableFeature> GetAvailablePlans(AvailabilityPlanStateParam planOptionsParam)
        {
            return GetResult(planOptionsParam);
        }

        public IEnumerable<AvailableFeature> GetAllPlans(AvailabilityPlanStateParam planOptionsParam)
        {
            return GetAllItems(planOptionsParam);
        }

        public IEnumerable<AvailableFeature> ApplyEligibility(IEnumerable<AvailableFeature> availableFeatures,
            IEnumerable<PlanEligibilityResult> eligibilityResults)
        {
            //TODO: can this be refactored with CoverAvailabilityProvider (that does kinda the same thing)
            foreach (var availableFeature in availableFeatures)
            {
                var planEligiblity = eligibilityResults.Single(r => r.PlanCode == availableFeature.Code);

                var inEligibleReason = planEligiblity.IneligibleReasons;

                yield return new AvailableFeature(
                    availableFeature.Code,
                    availableFeature.IsAvailable && planEligiblity.EligibleForPlan,
                    availableFeature.ReasonIfUnavailable.Concat(inEligibleReason));
            }
        }

        public override IEnumerable<IAvailability> GetAvailabilityItems(AvailabilityPlanStateParam planOptionsParam)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(planOptionsParam.BrandKey);
            return productDefinition.Plans;
        }

        public PlanAvailabilityProvider(IProductDefinitionBuilder productDefinitionBuilder,
            ISelectedProductPlanOptionsConverter selectedProductPlanOptionsConverter,
            INameLookupService nameLookupService)
            : base(productDefinitionBuilder, selectedProductPlanOptionsConverter, nameLookupService)
        {
        }
    }
}
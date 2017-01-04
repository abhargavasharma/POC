using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Models.Mappers;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Rules
{
    public interface ICoverAvailabilityProvider
    {
        IEnumerable<AvailableFeature> GetAvailableCovers(AvailabilityPlanStateParam planOptionsParam);
        IEnumerable<AvailableFeature> GetAllCovers(AvailabilityPlanStateParam planOptionsParam);

        IEnumerable<AvailableFeature> ApplyEligibility(IEnumerable<AvailableFeature> availableFeatures, IList<CoverEligibilityResult> eligibilityResults);
    }

    public class CoverAvailabilityProvider : BaseAvailabilityProvider, ICoverAvailabilityProvider
    {
        public override IEnumerable<IAvailability> GetAvailabilityItems(AvailabilityPlanStateParam planOptionsParam)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(planOptionsParam.BrandKey);
            return productDefinition.Plans.FirstOrDefault(
                    p => p.Code.Equals(planOptionsParam.PlanCode, StringComparison.OrdinalIgnoreCase))
                    .With(p => p.Covers);
        }

        public CoverAvailabilityProvider(IProductDefinitionBuilder productDefinitionBuilder,
            ISelectedProductPlanOptionsConverter selectedProductPlanOptionsConverter, INameLookupService nameLookupService)
            : base(productDefinitionBuilder, selectedProductPlanOptionsConverter, nameLookupService)
        {
        }

        public IEnumerable<AvailableFeature> GetAvailableCovers(AvailabilityPlanStateParam planOptionsParam)
        {
            return GetResult(planOptionsParam);
        }

        public IEnumerable<AvailableFeature> GetAllCovers(AvailabilityPlanStateParam planOptionsParam)
        {
            return GetAllItems(planOptionsParam);
        }

        public IEnumerable<AvailableFeature> ApplyEligibility(IEnumerable<AvailableFeature> availableFeatures,
            IList<CoverEligibilityResult> eligibilityResults)
        {
            //TODO: can this be refactored with PlanAvailabilityProvider (that does kinda the same thing)
            foreach (var availableFeature in availableFeatures)
            {
                var coverEligibility = eligibilityResults.Single(r => r.CoverCode == availableFeature.Code);

                yield return new AvailableFeature(
                    availableFeature.Code,
                    availableFeature.IsAvailable && coverEligibility.EligibleForCover,
                    availableFeature.ReasonIfUnavailable.Concat(coverEligibility.IneligibleReasons));
            }
        }
    }
}
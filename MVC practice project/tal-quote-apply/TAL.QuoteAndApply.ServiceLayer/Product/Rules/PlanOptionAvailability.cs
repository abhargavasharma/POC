using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Models.Mappers;
using TAL.QuoteAndApply.Product.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Rules
{
    public interface IPlanOptionAvailabilityProvider
    {
        IEnumerable<AvailableFeature> GetAvailableOptions(AvailabilityPlanStateParam planOptionsParam);
        IEnumerable<AvailableFeature> GetAllOptions(AvailabilityPlanStateParam planOptionsParam);
    }

    public class PlanOptionAvailabilityProvider : BaseAvailabilityProvider, IPlanOptionAvailabilityProvider
    {
        public PlanOptionAvailabilityProvider(IProductDefinitionBuilder productDefinitionBuilder,
            ISelectedProductPlanOptionsConverter selectedProductPlanOptionsConverter, INameLookupService nameLookupService)
            : base(productDefinitionBuilder, selectedProductPlanOptionsConverter, nameLookupService)
        {
        }

        public override IEnumerable<IAvailability> GetAvailabilityItems(AvailabilityPlanStateParam planOptionsParam)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(planOptionsParam.BrandKey);
            return productDefinition.Plans.FirstOrDefault(
                    p => p.Code.Equals(planOptionsParam.PlanCode, StringComparison.OrdinalIgnoreCase))
                    .With(pd => pd.Options);
        }

        public IEnumerable<AvailableFeature> GetAvailableOptions(AvailabilityPlanStateParam planOptionsParam)
        {
            return GetResult(planOptionsParam);
        }

        public IEnumerable<AvailableFeature> GetAllOptions(AvailabilityPlanStateParam planOptionsParam)
        {
            return GetAllItems(planOptionsParam);
        }
    }
}
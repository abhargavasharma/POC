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
    public interface IPlanRiderAvailabilityProvider
    {
        IEnumerable<AvailableFeature> GetAvailableRiders(AvailabilityPlanStateParam planOptionsParam);
        IEnumerable<AvailableFeature> GetAvailableRiderCovers(string riderCode, AvailabilityPlanStateParam planOptionsParam);
        IEnumerable<AvailableFeature> GetAvailableRiderOptions(string riderCode, AvailabilityPlanStateParam planOptionsParam);

        IEnumerable<AvailableFeature> GetAllRiders(AvailabilityPlanStateParam planOptionsParam);
        IEnumerable<AvailableFeature> GetAllRiderCovers(string riderCode, AvailabilityPlanStateParam planOptionsParam);
        IEnumerable<AvailableFeature> GetAllRiderOptions(string riderCode, AvailabilityPlanStateParam planOptionsParam);
    }

    public class PlanRiderAvailabilityProvider : BaseAvailabilityProvider, IPlanRiderAvailabilityProvider
    {
        public IEnumerable<AvailableFeature> GetAvailableRiders(AvailabilityPlanStateParam planOptionsParam)
        {
            return GetResult(planOptionsParam);
        }

        public IEnumerable<AvailableFeature> GetAvailableRiderCovers(string riderCode, AvailabilityPlanStateParam planOptionsParam)
        {
            var riderCovers = GetRiderCovers(riderCode, planOptionsParam);
            return GetResultForAvailabiltyItems(riderCovers, planOptionsParam);
        }

        public IEnumerable<AvailableFeature> GetAvailableRiderOptions(string riderCode, AvailabilityPlanStateParam planOptionsParam)
        {
            var riderOptions =  GetRiderOptions(riderCode, planOptionsParam);
            return GetResultForAvailabiltyItems(riderOptions, planOptionsParam);
        }

        public IEnumerable<AvailableFeature> GetAllRiders(AvailabilityPlanStateParam planOptionsParam)
        {
            return GetAllItems(planOptionsParam);
        }

        public IEnumerable<AvailableFeature> GetAllRiderCovers(string riderCode, AvailabilityPlanStateParam planOptionsParam)
        {
            var riderCovers = GetRiderCovers(riderCode, planOptionsParam);
            return GetAllAvailabiltyItems(riderCovers);
        }

        public IEnumerable<AvailableFeature> GetAllRiderOptions(string riderCode, AvailabilityPlanStateParam planOptionsParam)
        {
            var riderOptions = GetRiderOptions(riderCode, planOptionsParam);
            return GetAllAvailabiltyItems(riderOptions);
        }

        public override IEnumerable<IAvailability> GetAvailabilityItems(AvailabilityPlanStateParam planOptionsParam)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(planOptionsParam.BrandKey);
            return productDefinition.Plans.FirstOrDefault(
                    p => p.Code.Equals(planOptionsParam.PlanCode, StringComparison.OrdinalIgnoreCase))
                    .With(p => p.Riders);
        }

        private IEnumerable<IAvailability> GetRiderCovers(string riderCode, AvailabilityPlanStateParam planOptionsParam)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(planOptionsParam.BrandKey);
            return productDefinition.Plans
                .FirstOrDefault(plan => plan.Code.Equals(planOptionsParam.PlanCode, StringComparison.OrdinalIgnoreCase))
                .With(plan => plan.Riders)
                .With(rider => rider.FirstOrDefault(plan => plan.Code == riderCode))
                .With(rider => rider.Covers);
        }

        private IEnumerable<IAvailability> GetRiderOptions(string riderCode, AvailabilityPlanStateParam planOptionsParam)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(planOptionsParam.BrandKey);
            return productDefinition.Plans
                .FirstOrDefault(plan => plan.Code.Equals(planOptionsParam.PlanCode, StringComparison.OrdinalIgnoreCase))
                .With(plan => plan.Riders)
                .With(rider => rider.FirstOrDefault(plan => plan.Code == riderCode))
                .With(rider => rider.Options);
        }

        public PlanRiderAvailabilityProvider(IProductDefinitionBuilder productDefinitionBuilder,
            ISelectedProductPlanOptionsConverter selectedProductPlanOptionsConverter, INameLookupService nameLookupService)
            : base(productDefinitionBuilder, selectedProductPlanOptionsConverter, nameLookupService)
        {
        }
    }

}
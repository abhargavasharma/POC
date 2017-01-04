using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Models.Mappers;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Rules
{
    public interface IVariableAvailabilityProvider
    {
        IEnumerable<AvailableFeature> ApplyEligibility(IEnumerable<AvailableFeature> availableFeatures);
        IEnumerable<AvailableFeature> GetVariableAvailability(AvailabilityPlanStateParam planOptionsParam);

    }

    public class VariableAvailabilityProvider : BaseAvailabilityProvider, IVariableAvailabilityProvider
    {        

        public VariableAvailabilityProvider(IProductDefinitionBuilder productDefinitionBuilder,
            ISelectedProductPlanOptionsConverter selectedProductPlanOptionsConverter,
            INameLookupService nameLookupService)
            : base(productDefinitionBuilder, selectedProductPlanOptionsConverter, nameLookupService)
        {
        }

        public override IEnumerable<IAvailability> GetAvailabilityItems(AvailabilityPlanStateParam planOptionsParam)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(planOptionsParam.BrandKey);
            var plan = productDefinition.Plans.Single(p => p.Code == planOptionsParam.PlanCode);
            return plan.Variables;
        }

        public IEnumerable<AvailableFeature> ApplyEligibility(IEnumerable<AvailableFeature> availableFeatures)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AvailableFeature> GetVariableAvailability(AvailabilityPlanStateParam planOptionsParam)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(planOptionsParam.BrandKey);
            var plan = productDefinition.Plans.Single(p => p.Code == planOptionsParam.PlanCode);

            foreach (var variableDefinition in plan.Variables)
            {
                var variableAvailability = GetResultForAvailabiltyItems(new List<IAvailability> {variableDefinition},
                    planOptionsParam).Single();

                //Also get availablility for each Variable Option
                foreach (var variableOption in variableDefinition.Options)
                {
                    var variableOptionAvailability =
                        GetResultForAvailabiltyItems(new List<IAvailability> {variableOption}, planOptionsParam)
                            .Single();
                    variableAvailability.AddSubAvailableFeature(variableOptionAvailability);
                }
                yield return variableAvailability;
            }
        }
    }
}

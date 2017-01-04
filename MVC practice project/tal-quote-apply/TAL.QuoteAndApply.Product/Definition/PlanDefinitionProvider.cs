using System;
using System.Linq;
using System.Management.Instrumentation;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.Product.Definition
{
    public interface IPlanDefinitionProvider
    {
        PlanDefinition GetPlanByCode(string planCode, string brandKey);
    }

    public class PlanDefinitionProvider : IPlanDefinitionProvider
    {
        private readonly IProductDefinitionBuilder _productDefinitionBuilder;
        
        public PlanDefinitionProvider(IProductDefinitionBuilder productDefinitionBuilder)
        {
            _productDefinitionBuilder = productDefinitionBuilder;
        }

        public PlanDefinition GetPlanByCode(string planCode, string brandKey)
        {
            var productDefinition = _productDefinitionBuilder.BuildProductDefinition(brandKey);

            var productPlan = productDefinition.Plans
                .FirstOrDefault(plan => plan.Code.Equals(planCode, StringComparison.OrdinalIgnoreCase));

            if (productPlan != null)
            {
                return productPlan;
            }

            productPlan = productDefinition.Plans.SelectMany(p => p.Riders)
                    .FirstOrDefault(x => x.Code.Equals(planCode, StringComparison.OrdinalIgnoreCase));

            if (productPlan != null)
            {
                return productPlan;
            }

            throw new InstanceNotFoundException(string.Join("Could not locate plan '{0}' in product definition", planCode));
        }
    }
}

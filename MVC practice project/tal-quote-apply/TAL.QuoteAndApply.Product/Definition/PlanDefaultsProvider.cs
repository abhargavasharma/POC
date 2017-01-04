using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;

namespace TAL.QuoteAndApply.Product.Definition
{
    public interface IPlanDefaultsProvider
    {
        PlanDefaults GetPlanDefaultsByCode(string planCode);
    }

    public class PlanDefaultsProvider : IPlanDefaultsProvider
    {
        private readonly IPlanDefaultsBuilder _planDefaultsBuilder;

        private static IEnumerable<PlanDefaults> Plans;
        private static object LockObject;

        static PlanDefaultsProvider()
        {
            LockObject = new object();
        }

        public PlanDefaultsProvider(IPlanDefaultsBuilder planDefaultsBuilder)
        {
            _planDefaultsBuilder = planDefaultsBuilder;
        }


        public PlanDefaults GetPlanDefaultsByCode(string planCode)
        {
            if (Plans == null)
            {
                lock (LockObject)
                {
                    if (Plans == null)
                    {
                        Plans = _planDefaultsBuilder.BuildPlanDefaults();
                    }
                }
            }

            var productPlan = Plans.FirstOrDefault(plan => plan.Code.Equals(planCode, StringComparison.OrdinalIgnoreCase));

            if (productPlan != null)
            {
                return productPlan;
            }

            throw new InstanceNotFoundException(string.Join("Could not locate plan '{0}' in plan defaults", planCode));
        }
    }
}
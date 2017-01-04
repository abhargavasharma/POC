using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyPlanSpecificMappingProvider
    {
        RaisePolicyPlanSpecificMapping GetFor(string planCode);
    }

    public class RaisePolicyPlanSpecificMappingProvider : IRaisePolicyPlanSpecificMappingProvider
    {
        private readonly IDictionary<string, RaisePolicyPlanSpecificMapping> _mappings;

        public RaisePolicyPlanSpecificMappingProvider(IRaisePolicyPlanSpecificMappingBuilder builder)
        {
            _mappings = builder.Build();
        }

        public RaisePolicyPlanSpecificMapping GetFor(string planCode)
        {
            return _mappings[planCode];
        }
    }
}
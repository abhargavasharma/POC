using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Policy.Models.Converters
{
    public interface IPolicyProgressEnumConverter
    {
        PolicyProgress GetResidency(string policyProgress);
    }

    public class PolicyProgressEnumConverter : IPolicyProgressEnumConverter
    {
        public PolicyProgress GetResidency(string policyProgress)
        {
            if (!string.IsNullOrEmpty(policyProgress))
            {
                return (PolicyProgress)Enum.Parse(typeof(PolicyProgress), policyProgress, true);
            }

            return PolicyProgress.Unknown;
        }
    }
}

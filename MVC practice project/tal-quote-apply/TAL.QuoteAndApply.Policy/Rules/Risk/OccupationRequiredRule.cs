using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Risk
{
    public class OccupationRequiredRule : IRule<ISelectedOccupation>
    {
        private readonly string _key;

        public OccupationRequiredRule(string key)
        {
            _key = key;
        }
        
        public RuleResult IsSatisfiedBy(ISelectedOccupation selectedOccupation)
        {
            var isValid = //!string.IsNullOrEmpty(selectedOccupation.OccupationClass) // Occupation Class is not require for Life, Rpd and Ri/Ci
                          !string.IsNullOrEmpty(selectedOccupation.OccupationCode)
                          && !string.IsNullOrEmpty(selectedOccupation.OccupationTitle);

            return new RuleResult(_key, isValid);
        }
    }
}
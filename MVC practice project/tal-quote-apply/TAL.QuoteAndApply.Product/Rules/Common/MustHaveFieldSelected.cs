using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules.Common
{
    public class MustHaveFieldSelected : IRule<object>
    {
        public RuleResult IsSatisfiedBy(object obj)
        {
            var objectIsNull = obj == null;
            return new RuleResult(!objectIsNull);
        }
    }
}

using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Beneficiary
{
    public class BeneficiaryTitleSetRule : IRule<Title>
    {
        private readonly string _key;

        public BeneficiaryTitleSetRule(string key)
        {
            _key = key;
        }

        public RuleResult IsSatisfiedBy(Title target)
        {
            return RuleResult.ToResult(_key, target != Title.Unknown, "Title is required");
        }
    }
}
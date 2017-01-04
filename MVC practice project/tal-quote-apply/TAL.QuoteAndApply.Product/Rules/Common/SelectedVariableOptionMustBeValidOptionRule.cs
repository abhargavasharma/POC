using System.Linq;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules.Common
{
    public class SelectedVariableOptionMustBeValidOptionRule : IRule<object>
    {
        private readonly PlanVariablesDefinition _variableDefinition;

        public SelectedVariableOptionMustBeValidOptionRule(PlanVariablesDefinition variableDefinition)
        {
            _variableDefinition = variableDefinition;
        }

        public RuleResult IsSatisfiedBy(object target)
        {
            //Check that the selected value is actually a valid option of the Plan Variable
            var selectedOption = _variableDefinition.Options.SingleOrDefault(o => o.Value.Equals(target));
            if (selectedOption == null)
            {
                return new RuleResult(false);
            }

            return new RuleResult(true);
        }
    }
}

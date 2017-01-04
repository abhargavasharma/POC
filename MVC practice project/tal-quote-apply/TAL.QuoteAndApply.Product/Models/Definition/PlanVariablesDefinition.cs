using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public class PlanVariablesDefinition : IAvailability
    {
        public FeatureRule RuleDefinition { get; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public IList<PlanVariableOptionDefinition> Options { get; }
        public IReadOnlyList<AccessControlType> AllowEditingBy { get; private set; }

        public PlanVariablesDefinition(string code, string name, IList<PlanVariableOptionDefinition> options, FeatureRule ruleDefinition)
        {
            Code = code;
            Name = name;
            Options = options;
            RuleDefinition = ruleDefinition;
        }

        public PlanVariablesDefinition(string code, string name, IList<PlanVariableOptionDefinition> options,
            IReadOnlyList<AccessControlType> allowEditingBy, FeatureRule ruleDefinition)
            : this(code, name, options, ruleDefinition)
        {
            AllowEditingBy = allowEditingBy;
        }

    }

    public class PlanVariableOptionDefinition : IAvailability
    {
        public FeatureRule RuleDefinition { get; }

        public string Code => Value.ToString();

        public string Name { get; }
        public object Value { get; }

        public PlanVariableOptionDefinition(string name, object value, FeatureRule ruleDefinition)
        {
            Name = name;
            Value = value;
            RuleDefinition = ruleDefinition;
        }
    }
}

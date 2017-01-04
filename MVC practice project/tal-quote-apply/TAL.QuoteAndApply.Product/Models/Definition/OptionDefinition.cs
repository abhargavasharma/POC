using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public class OptionDefinition : IAvailability
    {
        public OptionDefinition(string code, string name, FeatureRule rule, bool? initiallySelected = null)
        {
            Name = name;
            Code = code;
            RuleDefinition = rule;
            InitiallySelected = initiallySelected;
        }

        public OptionDefinition(string code, string name, bool? initiallySelected = null)
        {
            Name = name;
            Code = code;
            InitiallySelected = initiallySelected;
        }

        public bool? InitiallySelected { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public FeatureRule RuleDefinition { get; set; }
    }
}

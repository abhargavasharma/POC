using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Product.Definition;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public class VariableResponse
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public object SelectedValue { get; set; } //TODO: this could move to a Selected attribute under object on VariableOptionResponse
        public IReadOnlyList<VariableOptionResponse> Options { get; set; }
        public IReadOnlyList<AccessControlType> AllowEditingBy { get; set; }

        public bool CustomerCanEdit => AllowEditingBy?.Contains(AccessControlType.Customer) ?? false;
    }

    public class VariableOptionResponse
    {
        public string Name { get; }
        public object Value { get; }
        public bool Enabled { get; }

        public VariableOptionResponse(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
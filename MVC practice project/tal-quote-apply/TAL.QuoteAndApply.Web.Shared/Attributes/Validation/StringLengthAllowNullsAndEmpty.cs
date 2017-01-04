using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TAL.QuoteAndApply.ServiceLayer.RulesProxy;

namespace TAL.QuoteAndApply.Web.Shared.Attributes.Validation
{
    public class StringLengthAllowNullsAndEmpty : ValidationAttribute
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }

            return DependencyResolver.Current.GetService<IGenericRules>().StringIsBetweenMinAndMaxLength((string)value, MinLength, MaxLength);
        }
    }
}
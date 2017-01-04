using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TAL.QuoteAndApply.ServiceLayer.RulesProxy;


namespace TAL.QuoteAndApply.Web.Shared.Attributes.Validation
{
    public class IsValidMobileOrLandlinePrefixRuleValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }

            return DependencyResolver.Current.GetService<IGenericRules>().IsValidMobileOrLandlinePrefixRuleValidation((string)value);
        }
    }
}

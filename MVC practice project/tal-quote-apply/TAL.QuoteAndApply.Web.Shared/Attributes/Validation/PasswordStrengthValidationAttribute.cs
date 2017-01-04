using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TAL.QuoteAndApply.ServiceLayer.RulesProxy;

namespace TAL.QuoteAndApply.Web.Shared.Attributes.Validation
{
    public class PasswordStrengthValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                //Allow no value to be valid (but make sure you check for requied as well)
                return true;
            }

            return DependencyResolver.Current.GetService<IGenericRules>().StringIsValidPasswordStrength((string)value);
        }
    }
}

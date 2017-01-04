using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TAL.QuoteAndApply.ServiceLayer.RulesProxy;

namespace TAL.QuoteAndApply.Web.Shared.Attributes.Validation
{
    public class NameValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            return DependencyResolver.Current.GetService<IGenericRules>().StringIsOnlyLettersSpacesHyphensRule((string) value);
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TAL.QuoteAndApply.ServiceLayer.Policy;

namespace TAL.QuoteAndApply.Web.Shared.Attributes.Validation
{
    public class PostcodeValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            var postcodeService = DependencyResolver.Current.GetService<IPostcodeService>();
            return postcodeService.IsValidPostcode(value.ToString());
        }
    }
}

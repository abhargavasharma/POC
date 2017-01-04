using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Infrastructure.Extensions;

namespace TAL.QuoteAndApply.Web.Shared.Attributes.Validation
{
    public class DateValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }

            var date = value.ToString().ToDateExcactDdMmYyyy();
            return date.HasValue;

        }
    }
}
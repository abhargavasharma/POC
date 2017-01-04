using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class CreateLoginRequest
    {
        [Required(ErrorMessage = "Password is required")]
        [PasswordStrengthValidation(ErrorMessage = "Your password must be at least 6 characters long, and include upper and lowercase letters.")]
        public string Password { get; set; }
    }
}
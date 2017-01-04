using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class LoginRequest
    {
        [RequiredIf("UseWindowsAuth", false, ErrorMessage = "User name is required")]
        public string UserName { get; set; }
        [RequiredIf("UseWindowsAuth", false, ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public bool UseWindowsAuth { get; set; }
    }
}
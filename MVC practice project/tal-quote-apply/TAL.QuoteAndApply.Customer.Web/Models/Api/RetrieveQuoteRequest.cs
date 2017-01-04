using System.ComponentModel.DataAnnotations;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class RetrieveQuoteRequest
    {
        [Required(ErrorMessage = "Reference number is required")]
        public string QuoteReference { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
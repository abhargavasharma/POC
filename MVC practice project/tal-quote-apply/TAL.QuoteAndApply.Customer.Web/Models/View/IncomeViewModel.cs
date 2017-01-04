using System.ComponentModel.DataAnnotations;
using TAL.QuoteAndApply.Web.Shared.Attributes.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Models.View
{
    public class IncomeViewModel
    {
        [Required(ErrorMessage = "Annual income is required")]
        public long? AnnualIncome { get; set; }
        
    }
}
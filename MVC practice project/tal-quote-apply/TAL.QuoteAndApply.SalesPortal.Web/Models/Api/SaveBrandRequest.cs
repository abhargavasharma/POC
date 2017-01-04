using System.ComponentModel.DataAnnotations;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class SaveBrandRequest
    {
        [Required]
        public string Brand { get; set; }
    }
}
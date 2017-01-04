using System.Collections.Generic;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting
{
    public class CustomerUnderwritingResponse
    {
        public int RiskId { get; set; }
        public string Status { get; set; } //TODO: replace with enum or something?
        public IList<QuestionResponse> Questions { get; set; }
        public IList<CategoryResponse> Categories { get; set; }
    }
}
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Underwriting;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class CoverDto : DbItem, ICover
    {
        public int CoverAmount { get; set; }
        public int PolicyId { get; set; }
        public int RiskId { get; set; }
        public string Code { get; set; }
        public bool Selected { get; set; }
        public int PlanId { get; set; }
        public decimal Premium { get; set; }
        public UnderwritingStatus UnderwritingStatus { get; set; }
    }
}

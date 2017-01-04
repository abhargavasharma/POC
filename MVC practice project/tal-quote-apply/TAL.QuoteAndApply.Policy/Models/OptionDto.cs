using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Policy.Data
{
    public class OptionDto : DbItem, IOption
    {
        public int RiskId { get; set; }
        public string Code { get; set; }
        public bool? Selected { get; set; }
        public int PlanId { get; set; }
    }
}
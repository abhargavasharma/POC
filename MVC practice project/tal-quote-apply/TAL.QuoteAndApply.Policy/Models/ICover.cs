using System;
using TAL.QuoteAndApply.DataModel.Underwriting;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface ICover
    {
        int Id { get; }
        int CoverAmount { get; set; }
        int PolicyId { get; set; }
        int RiskId { get; set; }
        string Code { get; set; }
        bool Selected { get; set; }
        int PlanId { get; set; }
        decimal Premium { get; set; }
        UnderwritingStatus UnderwritingStatus { get; set; }
    }
}
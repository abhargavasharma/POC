using System;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IPlan
    {
        int Id { get; }
        int CoverAmount { get; set; }
        int PolicyId { get; set; }
        int RiskId { get; set; }
        int? ParentPlanId { get; set; }
        string Code { get; set; }
        bool? LinkedToCpi { get; set; }
        bool? PremiumHoliday { get; set; }
        bool Selected { get; set; }
        decimal Premium { get; set; }
        decimal MultiCoverDiscount { get; set; }
        decimal MultiPlanDiscount { get; set; }
        decimal MultiPlanDiscountFactor { get; set; }
        PremiumType PremiumType { get; set; }

        int? WaitingPeriod { get; set; }
        int? BenefitPeriod { get; set; }
        OccupationDefinition OccupationDefinition { get; set; }
    }
}
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class DayOneAccidentBaseRateDto : DbItem
    {
        public string CoverCode { get; set; }
        public int BrandId { get; set; }
        public string PlanCode { get; set; }
        public int Age { get; set; }
        public PremiumType PremiumType { get; set; }
        public Gender Gender { get; set; }
        public int WaitingPeriod { get; set; }

        public decimal BaseRate { get; set; } 
    }
}
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class RiskOccupationDto : DbItem, IOccupationRatingFactors
    {
        public string OccupationCode { get; set; }
        public string OccupationTitle { get; set; }
        public string OccupationClass { get; set; }
        public int RiskId { get; set; }
        public string IndustryTitle { get; set; }
        public string IndustryCode { get; set; }
        public bool IsTpdAny { get; set; }
        public bool IsTpdOwn { get; set; }
        public decimal? TpdLoading { get; set; }
        public string PasCode { get; set; }
    }
}
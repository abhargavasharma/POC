namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IOccupationRatingFactors : ISelectedOccupation
    {
        int Id { get; set; }
        string IndustryTitle { get; set; }
        string IndustryCode { get; set; }
        bool IsTpdAny { get; set; }
        bool IsTpdOwn { get; set; }
        decimal? TpdLoading { get; set; }
        string PasCode { get; set; }
    }
}
namespace TAL.QuoteAndApply.Policy.Models
{
    public interface ISelectedOccupation
    {
        string OccupationCode { get; set; }
        string OccupationTitle { get; set; }
        string OccupationClass { get; set; }
    }
}
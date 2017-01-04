namespace TAL.QuoteAndApply.Policy.Models
{ 
    public class RiskProductDefinition
    {
        public int? MinimumAnnualIncomeDollars { get; set; }
        public bool AustralianResidencyRequired { get; set; }
        public int MinimumEntryAgeNextBirthday { get; set; }
        public int MaximumEntryAgeNextBirthday { get; set; }
    }
}

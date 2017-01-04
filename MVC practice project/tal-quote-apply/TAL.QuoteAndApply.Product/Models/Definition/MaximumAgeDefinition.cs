namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public class MaximumEntryAgeNextBirthdayForOccupationClassDefinition
    {
        public int MaximumAge { get; }
        public string OccupationClass { get; }

        public MaximumEntryAgeNextBirthdayForOccupationClassDefinition(int maximumAge, string occupationClass)
        {
            MaximumAge = maximumAge;
            OccupationClass = occupationClass;
        }
    }
}
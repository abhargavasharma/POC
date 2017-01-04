namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public class AgeRangeDefinition
    {
        public int LowerAge { get; }
        public int UpperAge { get; }

        public AgeRangeDefinition(int lowerAge, int upperAge)
        {
            LowerAge = lowerAge;
            UpperAge = upperAge;
        }
    }
    
    public class CoverAmountPercentageDefinition
    {
        public int PercentageOfIncome { get; }
        public int MaxCoverAmount { get; }

        public CoverAmountPercentageDefinition(int percentageOfIncome, int maxCoverAmount)
        {
            PercentageOfIncome = percentageOfIncome;
            MaxCoverAmount = maxCoverAmount;
        }
    }
}

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class FactorACalculatorInput
    {
        public bool IncludeInMultiPlanDiscount { get; }
        public decimal MultiPlanDiscountFactor { get; }
        public decimal LargeSumInsuredDiscountFactor { get; }
        public decimal OccupationFactor { get; }
        public decimal SmokingFactor { get; }
        public decimal IndemnityOptionFactor { get; }
        public decimal IncreasingClaimsOptionFactor { get; }
        public decimal WaitingPeriodFactor { get; }
        public decimal OccupationDefinitionFactor { get; }
        public decimal OccupationLoadingFactor { get; }

        public FactorACalculatorInput(bool includeInMultiPlanDiscount, decimal multiPlanDiscountFactor, 
            decimal largeSumInsuredDiscountFactor, decimal occupationFactor, decimal smokingFactor, 
            decimal indemnityOptionFactor, decimal increasingClaimsOptionFactor, decimal waitingPeriodFactor, 
            decimal occupationDefinitionFactor, decimal occupationLoadingFactor)
        {

            IncludeInMultiPlanDiscount = includeInMultiPlanDiscount;
            MultiPlanDiscountFactor = multiPlanDiscountFactor;
            LargeSumInsuredDiscountFactor = largeSumInsuredDiscountFactor;
            OccupationFactor = occupationFactor;
            SmokingFactor = smokingFactor;
            IndemnityOptionFactor = indemnityOptionFactor;
            IncreasingClaimsOptionFactor = increasingClaimsOptionFactor;
            WaitingPeriodFactor = waitingPeriodFactor;
            OccupationDefinitionFactor = occupationDefinitionFactor;
            OccupationLoadingFactor = occupationLoadingFactor;
        }
    }
}
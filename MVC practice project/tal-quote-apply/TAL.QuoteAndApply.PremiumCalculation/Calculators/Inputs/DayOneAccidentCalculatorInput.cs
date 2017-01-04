namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class DayOneAccidentCalculatorInput
    {
        public bool? DayOneAccidentSelected { get; }
        public decimal BaseRate { get; }
        public decimal OccupationFactor { get; }
        public decimal CoverAmount { get; }
        public decimal FactorB { get; }

        public DayOneAccidentCalculatorInput(bool? dayOneAccidentSelected, decimal baseRate, decimal occupationFactor, decimal coverAmount, decimal factorB)
        {
            DayOneAccidentSelected = dayOneAccidentSelected;
            BaseRate = baseRate;
            OccupationFactor = occupationFactor;
            CoverAmount = coverAmount;
            FactorB = factorB;
        }
    }
}
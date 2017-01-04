namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class CoverCalculatorInput
    {
        public decimal CoverAmount { get; }
        public decimal BaseRate { get; }
        public decimal FactorA { get; }
        public decimal FactorB { get; }
        public int DivionalFactor { get; }

        public CoverCalculatorInput(decimal coverAmount, decimal baseRate, decimal factorA, decimal factorB, int divionalFactor)
        {
            CoverAmount = coverAmount;
            BaseRate = baseRate;
            FactorA = factorA;
            FactorB = factorB;
            DivionalFactor = divionalFactor;
        }
    }
}
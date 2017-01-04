namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class FactorBCalculatorInput
    {
        public decimal PremiumReliefOptionFactor { get; }
        public decimal ModalFrequencyFactor { get; }

        public FactorBCalculatorInput(decimal premiumReliefOptionFactor, decimal modalFrequencyFactor)
        {
            PremiumReliefOptionFactor = premiumReliefOptionFactor;
            ModalFrequencyFactor = modalFrequencyFactor;
        }
    }
}
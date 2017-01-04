using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs
{
    public class LoadingCalculatorInput
    {
        public decimal BasePremium { get; }
        public decimal CoverAmount { get; }
        public decimal FactorB { get; }
        public ICoverFactors CoverCalculationRequest { get; }
        public decimal PercentageLoadingFactor { get; }
        public decimal PerMilleLoadingFactor { get; }

        public LoadingCalculatorInput(decimal basePremium, 
            decimal coverAmount, 
            decimal factorB, 
            ICoverFactors coverCalculationRequest,
            decimal percentageLoadingFactor,
            decimal perMilleLoadingFactor)
        {
            BasePremium = basePremium;
            CoverAmount = coverAmount;
            FactorB = factorB;
            CoverCalculationRequest = coverCalculationRequest;
            PercentageLoadingFactor = percentageLoadingFactor;
            PerMilleLoadingFactor = perMilleLoadingFactor;
        }
    }
}
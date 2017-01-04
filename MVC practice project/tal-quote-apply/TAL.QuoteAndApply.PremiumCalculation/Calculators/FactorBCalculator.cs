using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class FactorBCalculator
    {
        public static decimal Calculate(FactorBCalculatorInput input)
        {
            //start with premium relief factor
            var factor = input.PremiumReliefOptionFactor;

            //mutiply by modal frequency factor
            factor = factor * input.ModalFrequencyFactor;

            return factor;
        }
    }
}
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class FactorACalculator
    {
        public static decimal Calculate(FactorACalculatorInput input)
        {
            //start with 1
            var factor = 1m;

            if (input.IncludeInMultiPlanDiscount)
            {
                //multiply by multiplan discount factor
                factor = factor*input.MultiPlanDiscountFactor;
            }
           
            //mutiply by large sum insured discount factor
            factor = factor*input.LargeSumInsuredDiscountFactor;

            //TPD factors
            factor = factor * input.OccupationDefinitionFactor;
            factor = factor * input.OccupationLoadingFactor;

            //IP Factors
            factor = factor * input.OccupationFactor;
            factor = factor * input.SmokingFactor;
            factor = factor * input.IndemnityOptionFactor;
            factor = factor * input.IncreasingClaimsOptionFactor;
            factor = factor * input.WaitingPeriodFactor;

            return factor;
        }
    }
}
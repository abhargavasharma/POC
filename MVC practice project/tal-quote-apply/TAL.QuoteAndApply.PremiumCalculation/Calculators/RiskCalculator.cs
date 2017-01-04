using System.Linq;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class RiskCalculator
    {
        public static PremiumAndDiscount Calculate(RiskCalculatorInput riskCalculatorInput)
        {
            var riskPremiumWithDiscount = riskCalculatorInput.PlanResultsWithDiscounts.Sum(p => p.TotalPremium);
            var riskDiscount = riskCalculatorInput.PlanResultsWithDiscounts.Sum(p => p.MultiPlanDiscount);

            return new PremiumAndDiscount(riskPremiumWithDiscount, riskDiscount);
        }
    }
}

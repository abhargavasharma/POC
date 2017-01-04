using System;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class PlanCalculator
    {
        public static PlanPremiumAndDiscounts Calculate(PlanCalculatorInput input)
        {
            var multiCoverDiscount = MultiCoverDiscountCalculator.CalculateDiscount(input.MultiCoverDiscountCalculatorInput);

            //discount is returned as a negative number so "add" the -ve number to the summed premium
            var rawTotalPlanPremium = input.CoverCalculationResults.Sum(c => c.TotalPremium);
            var totalPremium = rawTotalPlanPremium + multiCoverDiscount;

            var rawBasePremium = input.CoverCalculationResults.Sum(c => c.BasePremium);
            var multiPlanDiscount = (rawBasePremium / input.MultiPlanDiscountFactor) - rawBasePremium;

            var multiPlanDiscountFactor = input.MultiPlanDiscountFactor;

            if (totalPremium == 0)
            {
                multiPlanDiscountFactor = 0;
            }

            return new PlanPremiumAndDiscounts(totalPremium.RoundToTwoDecimalPlaces(), multiPlanDiscount.RoundToTwoDecimalPlaces(), multiPlanDiscountFactor, Math.Abs(multiCoverDiscount.RoundToTwoDecimalPlaces()));
        }
    }
}
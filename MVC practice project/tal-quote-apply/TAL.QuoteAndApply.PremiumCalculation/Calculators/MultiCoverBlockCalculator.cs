using System;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Calculators
{
    public class MultiCoverBlockCalculator
    {
        public static PremiumAndDiscount Calculate(MultiCoverBlockCalculatorInput input)
        {
            var planDiscount = MultiCoverDiscountCalculator.CalculateDiscount(input.MultiCoverDiscountCalculatorInput);

            //discount is returned as a negative number so "add" the -ve number to the summed premium
            var multiCoverBlockPremium = input.IncludedCoverBlockPremiums.Sum() + planDiscount;

            return new PremiumAndDiscount(multiCoverBlockPremium.RoundToTwoDecimalPlaces(), Math.Abs(planDiscount.RoundToTwoDecimalPlaces()));
        }
    }
}
using System.Collections.Generic;
using NUnit.Framework;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Calculators
{
    [TestFixture]
    public class RiskCalculatorTests
    {
        [Test]
        public void Calculate_NoDiscounts()
        {
            var plansWithDiscounts = new List<PlanPremiumCalculationResult>
            {
                new PlanPremiumCalculationResult("ABC", 100, 0, 0, 0, null),
                new PlanPremiumCalculationResult("DEF", 100, 0, 0, 0, null),
            };

            var result = RiskCalculator.Calculate(new RiskCalculatorInput(plansWithDiscounts));

            Assert.That(result.Premium, Is.EqualTo(200));
            Assert.That(result.Discount, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_Discounts()
        {
            var plansWithDiscounts = new List<PlanPremiumCalculationResult>
            {
                new PlanPremiumCalculationResult("ABC", 99, 1, 0, 0, null),
                new PlanPremiumCalculationResult("DEF", 99, 1, 0, 0, null),
            };


            var result = RiskCalculator.Calculate(new RiskCalculatorInput(plansWithDiscounts));

            Assert.That(result.Premium, Is.EqualTo(198));
            Assert.That(result.Discount, Is.EqualTo(2));
        }
    }
}

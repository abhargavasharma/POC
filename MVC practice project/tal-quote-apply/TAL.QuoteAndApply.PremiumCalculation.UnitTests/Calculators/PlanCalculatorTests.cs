using System.Collections.Generic;
using NUnit.Framework;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Calculators
{
    [TestFixture]
    public class PlanCalculatorTests
    {
        [Test]
        public void Calculate_OneCover_PlanPremiumReturned()
        {
            var planDiscountInput = new MultiCoverDiscountCalculatorInput(.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(500m, false)
            });

            var coverPremiumResults = new List<CoverPremiumCalculationResult>
            {
                new CoverPremiumCalculationResult("ABC", 100, 0)
            };

            var planCalculatorInput = new PlanCalculatorInput(planDiscountInput, coverPremiumResults, 1);
            
            var result = PlanCalculator.Calculate(planCalculatorInput);

            Assert.That(result.Premium, Is.EqualTo(100));
            Assert.That(result.MultiCoverDiscount, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_OneCover_MultiPlanDiscount_PlanPremiumAndMultiPlanDiscountReturned()
        {
            var planDiscountInput = new MultiCoverDiscountCalculatorInput(.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(500m, false)
            });

            var coverPremiumResults = new List<CoverPremiumCalculationResult>
            {
                new CoverPremiumCalculationResult("ABC", 100, 0)
            };

            var planCalculatorInput = new PlanCalculatorInput(planDiscountInput, coverPremiumResults, .9m);

            var result = PlanCalculator.Calculate(planCalculatorInput);

            Assert.That(result.Premium, Is.EqualTo(100));
            Assert.That(result.MultiPlanDiscount, Is.EqualTo(11.11m));
            Assert.That(result.MultiPlanDiscountFactor, Is.EqualTo(.9m));
        }

        [Test]
        public void Calculate_MultipleCovers_NoCoverDiscount_TotalPlanPremiumReturned()
        {
            var planDiscountInput = new MultiCoverDiscountCalculatorInput(.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(500m, false)
            });

            var coverPremiumResults = new List<CoverPremiumCalculationResult>
            {
                new CoverPremiumCalculationResult("ABC", 100, 0),
                new CoverPremiumCalculationResult("DEF", 100, 0),
            };

            var planCalculatorInput = new PlanCalculatorInput(planDiscountInput, coverPremiumResults, 1);

            var result = PlanCalculator.Calculate(planCalculatorInput);

            Assert.That(result.Premium, Is.EqualTo(200));
            Assert.That(result.MultiCoverDiscount, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_MultipleCovers_CoverDiscount_TotalPlanPremiumLessCoverDiscountReturned()
        {
            var planDiscountInput = new MultiCoverDiscountCalculatorInput(-.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(500m, true),
                new MultiCoverDiscountCoverCalculatorInput(500m, true),
            });

            var coverPremiumResults = new List<CoverPremiumCalculationResult>
            {
                new CoverPremiumCalculationResult("ABC", 500, 0),
                new CoverPremiumCalculationResult("DEF", 500, 0),
            };

            var planCalculatorInput = new PlanCalculatorInput(planDiscountInput, coverPremiumResults, 1);

            var result = PlanCalculator.Calculate(planCalculatorInput);

            Assert.That(result.Premium, Is.EqualTo(500));
            Assert.That(result.MultiCoverDiscount, Is.EqualTo(500));
        }

        [Test]
        public void Calculate_PlanPremiumIsZero_ZeroReturnedAsMultiPlanDiscountFactor()
        {
            var planDiscountInput = new MultiCoverDiscountCalculatorInput(.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(0, false)
            });

            var coverPremiumResults = new List<CoverPremiumCalculationResult>
            {
                new CoverPremiumCalculationResult("ABC", 0, 0)
            };

            var planCalculatorInput = new PlanCalculatorInput(planDiscountInput, coverPremiumResults, 1);

            var result = PlanCalculator.Calculate(planCalculatorInput);

            Assert.That(result.Premium, Is.EqualTo(0));
            Assert.That(result.MultiCoverDiscount, Is.EqualTo(0));
        }

    }
}

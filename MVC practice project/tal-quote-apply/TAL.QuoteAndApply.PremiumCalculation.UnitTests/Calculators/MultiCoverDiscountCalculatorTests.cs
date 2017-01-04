using System.Collections.Generic;
using NUnit.Framework;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Calculators
{
    [TestFixture]
    public class MultiCoverDiscountCalculatorTests
    {
        [Test]
        public void CalculateDiscount_OneCoverNotIncludedInMultiCoverDiscount_ZeroDiscountReturned()
        {
            var input = new MultiCoverDiscountCalculatorInput(-.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(500m, false)
            });

            var result = MultiCoverDiscountCalculator.CalculateDiscount(input);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateDiscount_OneCoverIncludedInMultiCoverDiscount_ZeroDiscountReturned()
        {
            var input = new MultiCoverDiscountCalculatorInput(-.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(500m, true)
            });

            var result = MultiCoverDiscountCalculator.CalculateDiscount(input);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateDiscount_TwoCoversBothNotIncludedInMultiCoverDiscount_ZeroDiscountReturned()
        {
            var input = new MultiCoverDiscountCalculatorInput(-.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(500m, false),
                new MultiCoverDiscountCoverCalculatorInput(500m, false),
            });

            var result = MultiCoverDiscountCalculator.CalculateDiscount(input);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateDiscount_TwoCoversOneIncludedInMultiCoverDiscount_ZeroDiscountReturned()
        {
            var input = new MultiCoverDiscountCalculatorInput(-.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(500m, true),
                new MultiCoverDiscountCoverCalculatorInput(500m, false),
            });

            var result = MultiCoverDiscountCalculator.CalculateDiscount(input);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateDiscount_TwoCoversBothIncludedInMultiCoverDiscount_DiscountReturned()
        {
            var input = new MultiCoverDiscountCalculatorInput(-.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(500m, true),
                new MultiCoverDiscountCoverCalculatorInput(500m, true),
            });

            var result = MultiCoverDiscountCalculator.CalculateDiscount(input);

            Assert.That(result, Is.EqualTo(-500));
        }

        [Test]
        public void CalculateDiscount_ThreeCoversTwoIncludedInMultiCoverDiscount_DiscountReturned()
        {
            var input = new MultiCoverDiscountCalculatorInput(-.5m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(500m, true),
                new MultiCoverDiscountCoverCalculatorInput(500m, true),
                new MultiCoverDiscountCoverCalculatorInput(500m, false),
            });

            var result = MultiCoverDiscountCalculator.CalculateDiscount(input);

            Assert.That(result, Is.EqualTo(-500));
        }

        [Test]
        public void CalculateDiscount_RoundingCheck()
        {
            var input = new MultiCoverDiscountCalculatorInput(-.25m, new List<MultiCoverDiscountCoverCalculatorInput>
            {
                new MultiCoverDiscountCoverCalculatorInput(27.19m, true),
                new MultiCoverDiscountCoverCalculatorInput(54.39m, true)
            });

            var result = MultiCoverDiscountCalculator.CalculateDiscount(input);

            Assert.That(result, Is.EqualTo(-20.40m));
        }
    }
}

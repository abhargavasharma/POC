using NUnit.Framework;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Calculators
{
    [TestFixture]
    public class CoverCalculatorTests
    {
        [Test]
        public void Calculate_BaseRate()
        {
            const int baseRate = 2;

            var coverCalculatorInput = new CoverCalculatorInput(1, baseRate, 1, 1, 1);

            var result = CoverCalculator.Calculate(coverCalculatorInput);

            Assert.That(result, Is.EqualTo(baseRate));
        }

        [Test]
        public void Calculate_FactorA()
        {
            const int factorA = 2;

            var coverCalculatorInput = new CoverCalculatorInput(1, 1, factorA, 1, 1);

            var result = CoverCalculator.Calculate(coverCalculatorInput);

            Assert.That(result, Is.EqualTo(factorA));
        }

        [Test]
        public void Calculate_FactorB()
        {
            const int factorB = 2;

            var coverCalculatorInput = new CoverCalculatorInput(1, 1, 1, factorB, 1);

            var result = CoverCalculator.Calculate(coverCalculatorInput);

            Assert.That(result, Is.EqualTo(factorB));
        }

        [Test]
        public void Calculate_DivsionalFactor()
        {
            const int divisionalFactor = 10000;

            var coverCalculatorInput = new CoverCalculatorInput(divisionalFactor, 1, 1, 1, divisionalFactor);

            var result = CoverCalculator.Calculate(coverCalculatorInput);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_AllFactors()
        {
            var coverCalculatorInput = new CoverCalculatorInput(2, 2, 2, 2, 16);

            var result = CoverCalculator.Calculate(coverCalculatorInput);

            Assert.That(result, Is.EqualTo(1));
        }
    }
}

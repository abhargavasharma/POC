using NUnit.Framework;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Calculators
{
    [TestFixture]
    public class FactorBCalculatorTests
    {
        [Test]
        public void Calculate_PremiumReliefFactor()
        {
            const decimal premiumReliefFactor = .9m;

            var input = new FactorBCalculatorInput(premiumReliefFactor, 1);
            var result = FactorBCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(premiumReliefFactor));
        }

        [Test]
        public void Calculate_LargeSumInsuredDiscountFactor()
        {
            const decimal modalFrequencyFactor = 11;

            var input = new FactorBCalculatorInput(1, modalFrequencyFactor);
            var result = FactorBCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(modalFrequencyFactor));
        }

        [Test]
        public void Calculate_AllFactors()
        {
            const decimal premiumReliefFactor = .9m;
            const decimal modalFrequencyFactor = 11;
            var input = new FactorBCalculatorInput(premiumReliefFactor, modalFrequencyFactor);
            var result = FactorBCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(9.9m));
        }
    }
}
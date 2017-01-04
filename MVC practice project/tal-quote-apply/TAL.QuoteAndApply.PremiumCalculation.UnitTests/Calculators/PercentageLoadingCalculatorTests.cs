using NUnit.Framework;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Calculators
{
    [TestFixture]
    public class PercentageLoadingCalculatorTests
    {
        [Test]
        public void Calculate_ZeroPercentageLoading_ZeroPercentageLoadingFactor_ZeroReturned()
        {
            var input = new PercentageLoadingCalculatorInput(100, 0, 0);

            var result = PercentageLoadingCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_100PercentageLoading_ZeroPercentageLoadingFactor_100Returned()
        {
            var input = new PercentageLoadingCalculatorInput(100, 100, 0);

            var result = PercentageLoadingCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void Calculate_100PercentageLoading_PointOnePercentageLoadingFactor_100Returned()
        {
            var input = new PercentageLoadingCalculatorInput(100, 100, .01m);

            var result = PercentageLoadingCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(101));
        }
    }
}

using NUnit.Framework;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Calculators
{
    [TestFixture]
    public class PerMilleLoadingCalculatorTests
    {
        [Test]
        public void Calculate_ZeroPerMilleLoading_ZeroPerMilleLoadingFactor_ZeroReturned()
        {
            var input = new PerMilleLoadingCalculatorInput(100000, 0, 1, 0);

            var result = PerMilleLoadingCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_2PerMilleLoading_ZeroPerMilleLoading_FactorBIsOne_LoadingReturned()
        {
            var input = new PerMilleLoadingCalculatorInput(100000, 2, 0, 1);

            var result = PerMilleLoadingCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(18.18));
        }

        [Test]
        public void Calculate_2PerMilleLoading_PointElevenPerMilleLoading_FactorBIsOne_LoadingReturned()
        {
            var input = new PerMilleLoadingCalculatorInput(100000, 2, .11m, 1);

            var result = PerMilleLoadingCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(19.18));
        }

        [Test]
        public void Calculate_2PerMilleLoading_ZeroPerMilleLoading_FactorBIs11_LoadingReturned()
        {
            var input = new PerMilleLoadingCalculatorInput(100000, 2, 0, 11);

            var result = PerMilleLoadingCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(200));
        }
    }
}

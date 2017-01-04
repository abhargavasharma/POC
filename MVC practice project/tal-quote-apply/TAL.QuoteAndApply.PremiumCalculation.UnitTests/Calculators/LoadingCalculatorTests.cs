using NUnit.Framework;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Calculators
{
    [TestFixture]
    public class LoadingCalculatorTests
    {
        [TestCase(true, true, 118.18)]
        [TestCase(true, false, 100)]
        [TestCase(false, true, 18.18)]
        [TestCase(false, false, 0)]
        public void CalculateLoading_PerMilleAndPercentageIncluded(bool percentageLoadingSupported, bool perMilleLoadingSupported, decimal expectedLoading)
        {
            decimal percentageLoading = 100;
            decimal perMilleLoading = 2;
            var brandId = 1;

            var coverFactors = new CoverCalculationRequest("", true, true, percentageLoadingSupported, perMilleLoadingSupported, new Loadings(percentageLoading, perMilleLoading), brandId);

            var input = new LoadingCalculatorInput(100, 100000, 1, coverFactors, 0, 0);

            var result = LoadingCalculator.CalculateLoading(input);

            Assert.That(result, Is.EqualTo(expectedLoading));
        }
    }
}
using NUnit.Framework;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Calculators
{
    [TestFixture]
    public class DayOneAccidentCalculatorTests
    {
        [Test]
        public void Calculate_DayOneAccidentOptionNull_ZeroReturned()
        {
            var input = new DayOneAccidentCalculatorInput(null, 0, 1.16000m, 2000, 11);

            var result = DayOneAccidentCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_DayOneAccidentOptionFalse_ZeroReturned()
        {
            var input = new DayOneAccidentCalculatorInput(false, 0, 1.16000m, 2000, 11);

            var result = DayOneAccidentCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_BaseRateIsZero_ZeroReturned()
        {
            var input = new DayOneAccidentCalculatorInput(true, 0, 1.16000m, 2000, 11);

            var result = DayOneAccidentCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test, Description("From the algorithm spreadsheet")]
        public void Calculate_TestCaseOne()
        {
            var input = new DayOneAccidentCalculatorInput(true, 1.68000m, 1.16000m, 2000, 11);

            var result = DayOneAccidentCalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(42.87m));
        }

    }
}

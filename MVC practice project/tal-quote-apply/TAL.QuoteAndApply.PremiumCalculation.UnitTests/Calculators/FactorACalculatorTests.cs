using NUnit.Framework;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Calculators
{
    [TestFixture]
    public class FactorACalculatorTests
    {
        [Test]
        public void Calculate_IncludeInMultiPlanDiscount_MultiPlanDiscountFactor()
        {
            const decimal multiPlanDiscountFactor = .9m;

            var input = new FactorACalculatorInput(true, multiPlanDiscountFactor, 1, 1, 1, 1, 1, 1, 1, 1);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(multiPlanDiscountFactor));
        }

        [Test]
        public void Calculate_DoNotIncludeInMultiPlanDiscount_MultiPlanDiscountFactor()
        {
            const decimal multiPlanDiscountFactor = .9m;

            var input = new FactorACalculatorInput(false, multiPlanDiscountFactor, 1, 1, 1, 1, 1, 1, 1, 1);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void Calculate_LargeSumInsuredDiscountFactor()
        {
            const decimal largeSumInsuredDiscountFactor = .9m;

            var input = new FactorACalculatorInput(true, 1, largeSumInsuredDiscountFactor, 1, 1, 1, 1, 1, 1, 1);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(largeSumInsuredDiscountFactor));
        }

        [Test]
        public void Calculate_OccupationClassFactor()
        {
            const decimal occupationClassFactor = .9m;

            var input = new FactorACalculatorInput(true, 1, 1, occupationClassFactor, 1, 1, 1, 1, 1, 1);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(occupationClassFactor));
        }

        [Test]
        public void Calculate_SmokerFactor()
        {
            const decimal smokerFactor = .9m;

            var input = new FactorACalculatorInput(true, 1, 1, 1, smokerFactor, 1, 1, 1, 1, 1);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(smokerFactor));
        }

        [Test]
        public void Calculate_IndemnityOptionFactor()
        {
            const decimal indemnityOptionFactor = .9m;

            var input = new FactorACalculatorInput(true, 1, 1, 1, 1, indemnityOptionFactor, 1, 1, 1, 1);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(indemnityOptionFactor));
        }

        [Test]
        public void Calculate_IncreasingClaimsOptionFactor()
        {
            const decimal increasingClaimsOptionFactor = .9m;

            var input = new FactorACalculatorInput(true, 1, 1, 1, 1, 1, increasingClaimsOptionFactor, 1, 1, 1);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(increasingClaimsOptionFactor));
        }

        [Test]
        public void Calculate_WaitingPeriodFactor()
        {
            const decimal waitingPeriodFactor = .9m;

            var input = new FactorACalculatorInput(true, 1, 1, 1, 1, 1, 1, waitingPeriodFactor, 1, 1);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(waitingPeriodFactor));
        }

        [Test]
        public void Calculate_OccupationDefinitionFactor()
        {
            const decimal occDefinitionFactor = .9m;

            var input = new FactorACalculatorInput(true, 1, 1, 1, 1, 1, 1, 1, occDefinitionFactor, 1);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(occDefinitionFactor));
        }

        [Test]
        public void Calculate_OccupationLoadingFactor()
        {
            const decimal occLoadingFactor = .9m;

            var input = new FactorACalculatorInput(true, 1, 1, 1, 1, 1, 1, 1, 1, occLoadingFactor);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(occLoadingFactor));
        }

        [Test]
        public void Calculate_IncludeInMultiPlanDiscount_AllFactors()
        {
            const decimal largeSumInsuredDiscountFactor = .9m;
            const decimal multiPlanDiscountFactor = .9m;

            const decimal occupationClassFactor = .9m;
            const decimal smokerFactor = .9m;
            const decimal indemnityOptionFactor = .9m;
            const decimal increasingClaimsOptionFactor = .9m;
            const decimal waitingPeriodFactor = .9m;

            const decimal occDefinitionFactor = .9m;
            const decimal occLoadingFactor = .9m;

            var input = new FactorACalculatorInput(true, multiPlanDiscountFactor, largeSumInsuredDiscountFactor, occupationClassFactor, smokerFactor, indemnityOptionFactor, increasingClaimsOptionFactor, waitingPeriodFactor, occDefinitionFactor, occLoadingFactor);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(0.387420489m));
        }

        [Test]
        public void Calculate_DoNotIncludeInMultiPlanDiscount_AllFactors()
        {
            const decimal largeSumInsuredDiscountFactor = .9m;
            const decimal multiPlanDiscountFactor = .9m;

            const decimal occupationClassFactor = .9m;
            const decimal smokerFactor = .9m;
            const decimal indemnityOptionFactor = .9m;
            const decimal increasingClaimsOptionFactor = .9m;
            const decimal waitingPeriodFactor = .9m;

            const decimal occDefinitionFactor = .9m;
            const decimal occLoadingFactor = .9m;

            var input = new FactorACalculatorInput(false, multiPlanDiscountFactor, largeSumInsuredDiscountFactor, occupationClassFactor, smokerFactor, indemnityOptionFactor, increasingClaimsOptionFactor, waitingPeriodFactor, occDefinitionFactor, occLoadingFactor);
            var result = FactorACalculator.Calculate(input);

            Assert.That(result, Is.EqualTo(0.43046721m));
        }
    }
}
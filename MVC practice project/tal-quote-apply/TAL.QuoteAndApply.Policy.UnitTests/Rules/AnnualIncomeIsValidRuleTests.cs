using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules
{
    [TestFixture]
    public class AnnualIncomeIsValidRuleTests
    {


        [Test]
        public void IsSatisfiedBy_PlanDefinitionDoesNotHaveMinimumAnnualIncomeDollars_IsSatisfied()
        {
            var riskProductDefinition = new RiskProductDefinition();
            riskProductDefinition.MinimumAnnualIncomeDollars = null;

            var annualIncomeIsValidRule = new AnnualIncomeIsValidRule(riskProductDefinition, "");
            var result = annualIncomeIsValidRule.IsSatisfiedBy(100000);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_AnnualIncomeOverMinimumAnnualIncomeDollars_IsSatisfied()
        {
            var riskProductDefinition = new RiskProductDefinition();
            riskProductDefinition.MinimumAnnualIncomeDollars = 100000;

            var annualIncomeIsValidRule = new AnnualIncomeIsValidRule(riskProductDefinition, "");
            var result = annualIncomeIsValidRule.IsSatisfiedBy(100001);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_AnnualIncomeEqualMinimumAnnualIncomeDollars_IsSatisfied()
        {
            var riskProductDefinition = new RiskProductDefinition();
            riskProductDefinition.MinimumAnnualIncomeDollars = 100000;

            var annualIncomeIsValidRule = new AnnualIncomeIsValidRule(riskProductDefinition, "");
            var result = annualIncomeIsValidRule.IsSatisfiedBy(100000);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_AnnualIncomeUnderMinimumAnnualIncomeDollars_IsBroken()
        {
            var riskProductDefinition = new RiskProductDefinition();
            riskProductDefinition.MinimumAnnualIncomeDollars = 100000;

            var annualIncomeIsValidRule = new AnnualIncomeIsValidRule(riskProductDefinition, "");
            var result = annualIncomeIsValidRule.IsSatisfiedBy(99999);

            Assert.That(result.IsBroken, Is.True);
        }
    }
}

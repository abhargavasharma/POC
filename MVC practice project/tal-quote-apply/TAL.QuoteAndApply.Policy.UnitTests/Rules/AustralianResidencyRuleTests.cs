using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules
{
    [TestFixture]
    public class AustralianResidencyRuleTests
    {
        [Test]
        public void IsSatisfiedBy_RiskPlanDefinitionAustralianResidencyRequiredFalse_IsSatisfied()
        {
            var definition = new RiskProductDefinition();
            definition.AustralianResidencyRequired = false;

            var annualIncomeIsValidRule = new AustralianResidencyRule(definition, "");
            var result = annualIncomeIsValidRule.IsSatisfiedBy(ResidencyStatus.Australian);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_AustralianResidencyRequired_ResdiencyStatusAustralian_IsSatisfied()
        {
            var definition = new RiskProductDefinition();
            definition.AustralianResidencyRequired = true;

            var annualIncomeIsValidRule = new AustralianResidencyRule(definition, "");
            var result = annualIncomeIsValidRule.IsSatisfiedBy(ResidencyStatus.Australian);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_AustralianResidencyRequired_ResdiencyStatusNonAustralian_IsBroken()
        {
            var definition = new RiskProductDefinition();
            definition.AustralianResidencyRequired = true;

            var annualIncomeIsValidRule = new AustralianResidencyRule(definition, "");
            var result = annualIncomeIsValidRule.IsSatisfiedBy(ResidencyStatus.NonAustralian);

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_AustralianResidencyRequired_ResdiencyStatusUnkown_IsBroken()
        {
            var definition = new RiskProductDefinition();
            definition.AustralianResidencyRequired = true;

            var annualIncomeIsValidRule = new AustralianResidencyRule(definition, "");
            var result = annualIncomeIsValidRule.IsSatisfiedBy(ResidencyStatus.Unknown);

            Assert.That(result.IsBroken, Is.True);
        }
    }
}
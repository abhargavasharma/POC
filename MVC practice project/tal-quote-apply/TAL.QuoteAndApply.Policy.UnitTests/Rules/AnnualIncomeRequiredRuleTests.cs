using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Rules.Risk;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules
{
    [TestFixture]
    public class AnnualIncomeRequiredRuleTests
    {
        [Test]
        public void IsSatisfiedBy_AnnualIncomeIsNegative_IsBroken()
        {
            var annualIncomeIsValidRule = new AnnualIncomeRequiredRule("");
            var result = annualIncomeIsValidRule.IsSatisfiedBy(-1);

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_AnnualIncomeIsProvided_IsSatisfied()
        {
            var annualIncomeIsValidRule = new AnnualIncomeRequiredRule("");
            var result = annualIncomeIsValidRule.IsSatisfiedBy(100000);

            Assert.That(result.IsSatisfied, Is.True);
        }
    }
}
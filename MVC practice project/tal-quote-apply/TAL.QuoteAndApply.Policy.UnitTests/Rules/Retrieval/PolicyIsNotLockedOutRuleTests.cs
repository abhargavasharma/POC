using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Retrieval;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules.Retrieval
{
    [TestFixture]
    public class PolicyIsNotLockedOutRuleTests
    {
        [TestCase(PolicySaveStatus.CreatedLogin)]
        [TestCase(PolicySaveStatus.NotSaved)]
        [TestCase(PolicySaveStatus.PersonalDetailsEntered)]
        public void IsSatisfiedBy_PolicyNotLockedOutStatus_IsSatisfied(PolicySaveStatus policySaveStatus)
        {
            var policy = new PolicyDto { SaveStatus = policySaveStatus };
            var rule = new PolicyIsNotLockedOutRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsSatisfied);
        }

        [Test]
        public void IsSatisfiedBy_PolicyLockedOutStatus_IsBroken()
        {
            var policy = new PolicyDto { SaveStatus = PolicySaveStatus.LockedOutDueToRefer };
            var rule = new PolicyIsNotLockedOutRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsBroken);
        }
    }
}
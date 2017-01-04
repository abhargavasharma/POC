using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Retrieval;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules.Retrieval
{
    [TestFixture]
    public class PolicyIsNotReferredRuleTests
    {
        [Test]
        public void IsSatisfiedBy_PolicyIsIncompleteStatus_IsSatisfied()
        {
            var policy = new PolicyDto { Status = PolicyStatus.Incomplete };
            var rule = new PolicyIsNotReferredRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsSatisfied);
        }

        [Test]
        public void IsSatisfiedBy_PolicyIsRaisedToPolicyAdminSystemStatus_IsSatisfied()
        {
            var policy = new PolicyDto { Status = PolicyStatus.RaisedToPolicyAdminSystem };
            var rule = new PolicyIsNotReferredRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsSatisfied);
        }

        [Test]
        public void IsSatisfiedBy_PolicyIsReadyForInforceStatus_IsSatisfied()
        {
            var policy = new PolicyDto { Status = PolicyStatus.ReadyForInforce };
            var rule = new PolicyIsNotReferredRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsSatisfied);
        }

        [Test]
        public void IsSatisfiedBy_PolicyIsReferredToUnderwriterStatus_IsBroken()
        {
            var policy = new PolicyDto { Status = PolicyStatus.ReferredToUnderwriter };
            var rule = new PolicyIsNotReferredRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsBroken);
        }
    }
}

using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Retrieval;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules.Retrieval
{
    [TestFixture]
    public class PolicyIsNotSubmittedTests
    {
        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem)]
        [TestCase(PolicyStatus.ReadyForInforce)]
        [TestCase(PolicyStatus.FailedDuringPolicyAdminSystemLoad)]
        [TestCase(PolicyStatus.FailedToSendToPolicyAdminSystem)]
        [TestCase(PolicyStatus.Inforce)]
        public void IsSatisfiedBy_PolicySubmittedStatus_IsBroken(PolicyStatus submittedStatus)
        {
            var policy = new PolicyDto { Status = submittedStatus };
            var rule = new PolicyIsNotSubmittedRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsBroken);
        }

        [TestCase(PolicyStatus.ReferredToUnderwriter)]
        [TestCase(PolicyStatus.Incomplete)]
        public void IsSatisfiedBy_PolicyIsNotSubmittedStatus_IsSatisfied(PolicyStatus notSubmittedStatus)
        {
            var policy = new PolicyDto { Status = notSubmittedStatus };
            var rule = new PolicyIsNotSubmittedRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsSatisfied);
        }
    }
}

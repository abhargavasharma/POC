using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Retrieval;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules.Retrieval
{
    [TestFixture]
    public class PolicyIsSavedRuleTests
    {
        [Test]
        public void IsSatisfiedBy_PolicyIsNotSavedStatus_IsBroken()
        {
            var policy = new PolicyDto {SaveStatus = PolicySaveStatus.NotSaved};
            var rule = new PolicyIsSavedRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsBroken);
        }

        [Test]
        public void IsSatisfiedBy_PolicyIsPersonalDetailsSaveStatus_IsBroken()
        {
            var policy = new PolicyDto { SaveStatus = PolicySaveStatus.PersonalDetailsEntered };
            var rule = new PolicyIsSavedRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsBroken);
        }

        [Test]
        public void IsSatisfiedBy_PolicyHasLoginCreatedStatus_IsSatisfied()
        {
            var policy = new PolicyDto { SaveStatus = PolicySaveStatus.CreatedLogin };
            var rule = new PolicyIsSavedRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsSatisfied);
        }

        [Test]
        public void IsSatisfiedBy_PolicyHasLockedOutDueToReferStatus_IsSatisfied()
        {
            var policy = new PolicyDto { SaveStatus = PolicySaveStatus.LockedOutDueToRefer };
            var rule = new PolicyIsSavedRule();
            var result = rule.IsSatisfiedBy(policy);

            Assert.That(result.IsSatisfied);
        }
    }
}

using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Cover;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules.Cover
{
    [TestFixture]
    public class SelectedCoverNotUnderwritingDeclinedRuleTests
    {
        [Test]
        public void IsSatisfied_CoverNotSelected_IsSatisfied()
        {
            var cover = new CoverDto {Selected = false};

            var rule = new SelectedCoverNotUnderwritingDeclinedRule();
            var result = rule.IsSatisfiedBy(cover);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfied_DeclineStatus_IsBroken()
        {
            var cover = new CoverDto { Selected = true, UnderwritingStatus = UnderwritingStatus.Decline};

            var rule = new SelectedCoverNotUnderwritingDeclinedRule();
            var result = rule.IsSatisfiedBy(cover);

            Assert.That(result.IsBroken, Is.True);
        }

        [TestCase(UnderwritingStatus.Accept)]
        [TestCase(UnderwritingStatus.Defer)]
        [TestCase(UnderwritingStatus.Incomplete)]
        [TestCase(UnderwritingStatus.MoreInfo)]
        [TestCase(UnderwritingStatus.Refer)]
        public void IsSatisfied_NotDeclineStatus_IsStatisfied(UnderwritingStatus uwStatus)
        {
            var cover = new CoverDto
            {
                Selected = true,
                UnderwritingStatus = uwStatus
            };

            var rule = new SelectedCoverNotUnderwritingDeclinedRule();
            var result = rule.IsSatisfiedBy(cover);

            Assert.That(result.IsSatisfied, Is.True);
        }
    }
}

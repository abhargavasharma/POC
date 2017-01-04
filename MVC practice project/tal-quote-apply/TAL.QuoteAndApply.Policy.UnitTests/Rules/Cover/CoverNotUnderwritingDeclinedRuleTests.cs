using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Cover;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules.Cover
{
    public class CoverNotUnderwritingDeclinedRuleTests
    {

        [TestCase(true, UnderwritingStatus.Accept, Result = true)]
        [TestCase(true, UnderwritingStatus.Defer, Result = true)]
        [TestCase(true, UnderwritingStatus.Incomplete, Result = true)]
        [TestCase(true, UnderwritingStatus.MoreInfo, Result = true)]
        [TestCase(true, UnderwritingStatus.Refer, Result = true)]
        [TestCase(true, UnderwritingStatus.Decline, Result = false)]
        [TestCase(false, UnderwritingStatus.Accept, Result = true)]
        [TestCase(false, UnderwritingStatus.Defer, Result = true)]
        [TestCase(false, UnderwritingStatus.Incomplete, Result = true)]
        [TestCase(false, UnderwritingStatus.MoreInfo, Result = true)]
        [TestCase(false, UnderwritingStatus.Refer, Result = true)]
        [TestCase(false, UnderwritingStatus.Decline, Result = false)]
        public bool IsSatisfied_NotDeclineStatus_IsStatisfied(bool coverIsSelected, UnderwritingStatus uwStatus)
        {
            var cover = new CoverDto
            {
                Selected = coverIsSelected,
                UnderwritingStatus = uwStatus
            };

            var rule = new CoverNotUnderwritingDeclinedRule();
            var result = rule.IsSatisfiedBy(cover);

            return result.IsSatisfied;
        }
    }
}

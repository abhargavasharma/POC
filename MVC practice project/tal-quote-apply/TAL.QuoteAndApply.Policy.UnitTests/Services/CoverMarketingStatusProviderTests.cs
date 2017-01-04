using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Service.MarketingStatus;

namespace TAL.QuoteAndApply.Policy.UnitTests.Services
{
    [TestFixture]
    public class CoverMarketingStatusProviderTests
    {
        [TestCase(false, UnderwritingStatus.Accept, false, MarketingStatus.Off)]
        [TestCase(false, UnderwritingStatus.Decline, false, MarketingStatus.Off)]
        [TestCase(false, UnderwritingStatus.Defer, false, MarketingStatus.Off)]
        [TestCase(false, UnderwritingStatus.Incomplete, false, MarketingStatus.Off)]
        [TestCase(false, UnderwritingStatus.MoreInfo, false, MarketingStatus.Off)]
        [TestCase(false, UnderwritingStatus.Refer, false, MarketingStatus.Off)]
        [TestCase(true, UnderwritingStatus.Accept, false, MarketingStatus.Ineligible)]
        [TestCase(true, UnderwritingStatus.Decline, false, MarketingStatus.Decline)]
        [TestCase(true, UnderwritingStatus.Defer, false, MarketingStatus.Ineligible)]
        [TestCase(true, UnderwritingStatus.Incomplete, false, MarketingStatus.Ineligible)]
        [TestCase(true, UnderwritingStatus.MoreInfo, false, MarketingStatus.Ineligible)]
        [TestCase(true, UnderwritingStatus.Refer, false, MarketingStatus.Ineligible)]
        [TestCase(false, UnderwritingStatus.Accept, true, MarketingStatus.Off)]
        [TestCase(false, UnderwritingStatus.Decline, true, MarketingStatus.Off)]
        [TestCase(false, UnderwritingStatus.Defer, true, MarketingStatus.Off)]
        [TestCase(false, UnderwritingStatus.Incomplete, true, MarketingStatus.Off)]
        [TestCase(false, UnderwritingStatus.MoreInfo, true, MarketingStatus.Off)]
        [TestCase(false, UnderwritingStatus.Refer, true, MarketingStatus.Off)]
        [TestCase(true, UnderwritingStatus.Accept, true, MarketingStatus.Accept)]
        [TestCase(true, UnderwritingStatus.Decline, true, MarketingStatus.Decline)]
        [TestCase(true, UnderwritingStatus.Defer, true, MarketingStatus.Refer)]
        [TestCase(true, UnderwritingStatus.Incomplete, true, MarketingStatus.Lead)]
        [TestCase(true, UnderwritingStatus.MoreInfo, true, MarketingStatus.Lead)]
        [TestCase(true, UnderwritingStatus.Refer, true, MarketingStatus.Refer)]
        public void UpdateCoverMarketingStatus_UpdatedWithNewMarketingStatus_CorrectCoverMarketingStatusResult(
            bool coverSelected, UnderwritingStatus coverUnderwritingStatus, bool coverEligibile, MarketingStatus coverMarketingStatus)
        {
            //Arrange
            var service = new CoverMarketingStatusProvider();

            //Act
            var result = service.GetCoverMarketingStatus(coverSelected, coverEligibile, coverUnderwritingStatus);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(coverMarketingStatus));
        }
    }
}

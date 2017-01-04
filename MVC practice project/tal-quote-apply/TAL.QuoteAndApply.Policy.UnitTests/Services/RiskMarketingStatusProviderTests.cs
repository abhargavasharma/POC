using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Service.MarketingStatus;

namespace TAL.QuoteAndApply.Policy.UnitTests.Services
{
    [TestFixture]
    public class RiskMarketingStatusProviderTests
    {
        [TestCase(MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead)]
        [TestCase(MarketingStatus.Refer, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Refer)]
        [TestCase(MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Refer, MarketingStatus.Refer)]
        [TestCase(MarketingStatus.Ineligible, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead)]
        [TestCase(MarketingStatus.Ineligible, MarketingStatus.Ineligible, MarketingStatus.Ineligible, MarketingStatus.Lead, MarketingStatus.Lead)]
        [TestCase(MarketingStatus.Decline, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead)]
        [TestCase(MarketingStatus.Decline, MarketingStatus.Decline, MarketingStatus.Decline, MarketingStatus.Lead, MarketingStatus.Lead)]
        [TestCase(MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Lead, MarketingStatus.Lead)]
        [TestCase(MarketingStatus.Decline, MarketingStatus.Decline, MarketingStatus.Decline, MarketingStatus.Decline, MarketingStatus.Decline)]
        [TestCase(MarketingStatus.Ineligible, MarketingStatus.Ineligible, MarketingStatus.Ineligible, MarketingStatus.Ineligible, MarketingStatus.Ineligible)]
        [TestCase(MarketingStatus.Ineligible, MarketingStatus.Refer, MarketingStatus.Ineligible, MarketingStatus.Ineligible, MarketingStatus.Refer)]
        [TestCase(MarketingStatus.Refer, MarketingStatus.Refer, MarketingStatus.Refer, MarketingStatus.Refer, MarketingStatus.Refer)]
        [TestCase(MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Off)]
        [TestCase(MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Ineligible, MarketingStatus.Ineligible)]
        [TestCase(MarketingStatus.Off, MarketingStatus.Ineligible, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Ineligible)]
        [TestCase(MarketingStatus.Off, MarketingStatus.Refer, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Refer)]
        [TestCase(MarketingStatus.Off, MarketingStatus.Accept, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Lead)]
        [TestCase(MarketingStatus.Off, MarketingStatus.Lead, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Lead)]
        [TestCase(MarketingStatus.Decline, MarketingStatus.Decline, MarketingStatus.Decline, MarketingStatus.Refer, MarketingStatus.Refer)]
        [TestCase(MarketingStatus.Decline, MarketingStatus.Decline, MarketingStatus.Decline, MarketingStatus.Ineligible, MarketingStatus.Ineligible)]
        public void UpdateRiskMarketingStatus_UpdatedWithNewMarketingStatus_CorrectRiskMarketingStatusResult(
            MarketingStatus lifeStatus, MarketingStatus tpdStatus, MarketingStatus riStatus, MarketingStatus ipStatus, MarketingStatus coverMarketingStatus)
        {
            //Arrange
            var service = new RiskMarketingStatusProvider();

            var plansMarketingStatusList = new[] { lifeStatus, tpdStatus, riStatus, ipStatus };

            //Act
            var result = service.GetRiskMarketingStatus(plansMarketingStatusList);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(coverMarketingStatus));
        }
    }
}

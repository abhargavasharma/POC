using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Service.MarketingStatus;

namespace TAL.QuoteAndApply.Policy.UnitTests.Services
{
    [TestFixture]
    public class PlanMarketingStatusProviderTests
    {
        [TestCase(true, true, MarketingStatus.Refer, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Refer)]
        [TestCase(true, true, MarketingStatus.Decline, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Refer)]
        //Marketing status is always lead except with a refer or decline combined with a lead or accept - where it is refer as above
        [TestCase(true, true, MarketingStatus.Ineligible, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead)]
        [TestCase(true, true, MarketingStatus.Accept, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead)]
        [TestCase(true, true, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead)]
        [TestCase(true, true, MarketingStatus.Off, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead)]
        [TestCase(true, true, MarketingStatus.Unknown, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead)]
        //Marketing Status is always ineligible when plan is ineligible
        [TestCase(true, false, MarketingStatus.Ineligible, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Ineligible)]
        [TestCase(true, false, MarketingStatus.Accept, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Ineligible)]
        [TestCase(true, false, MarketingStatus.Decline, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Ineligible)]
        [TestCase(true, false, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Ineligible)]
        [TestCase(true, false, MarketingStatus.Off, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Ineligible)]
        [TestCase(true, false, MarketingStatus.Refer, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Ineligible)]
        [TestCase(true, false, MarketingStatus.Unknown, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Ineligible)]
        //Marketing Status is always off when plan is not selected
        [TestCase(false, true, MarketingStatus.Ineligible, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, true, MarketingStatus.Accept, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, true, MarketingStatus.Decline, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, true, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, true, MarketingStatus.Off, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, true, MarketingStatus.Refer, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, true, MarketingStatus.Unknown, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, false, MarketingStatus.Ineligible, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, false, MarketingStatus.Accept, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, false, MarketingStatus.Decline, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, false, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, false, MarketingStatus.Off, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, false, MarketingStatus.Refer, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        [TestCase(false, false, MarketingStatus.Unknown, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off)]
        //Line 27 of spreadsheet
        [TestCase(true, true, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Decline, MarketingStatus.Refer)]
        //Line 36 of spreadsheet
        [TestCase(true, true, MarketingStatus.Lead, MarketingStatus.Lead, MarketingStatus.Off, MarketingStatus.Lead)]
        //Line 45 of spreadsheet
        [TestCase(true, false, MarketingStatus.Decline, MarketingStatus.Off, MarketingStatus.Ineligible, MarketingStatus.Ineligible)]
        //Line 54 of spreadsheet
        [TestCase(true, true, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Ineligible, MarketingStatus.Ineligible)]
        //Line 63 of spreadsheet
        [TestCase(true, true, MarketingStatus.Decline, MarketingStatus.Decline, MarketingStatus.Ineligible, MarketingStatus.Decline)]
        //Line 72 of spreadsheet
        [TestCase(true, true, MarketingStatus.Accept, MarketingStatus.Accept, MarketingStatus.Accept, MarketingStatus.Lead)]
        //Line 81 of spreadsheet
        [TestCase(false, true, MarketingStatus.Accept, MarketingStatus.Accept, MarketingStatus.Accept, MarketingStatus.Off)]
        [TestCase(false, true, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Off, MarketingStatus.Off)]
        public void UpdatePlanMarketingStatus_UpdatedWithNewMarketingStatus_CorrectPlanMarketingStatusResult(
            bool planSelected, bool planEligibile, MarketingStatus firstSCovertatus, MarketingStatus secondCoverStatus, MarketingStatus thirdStatus, MarketingStatus coverMarketingStatus)
        {
            //Arrange
            var service = new PlanMarketingStatusProvider();

            //Act
            var result = service.GetPlanMarketingStatus(planSelected, planEligibile, new[] { firstSCovertatus, secondCoverStatus, thirdStatus });

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(coverMarketingStatus));
        }
    }
}

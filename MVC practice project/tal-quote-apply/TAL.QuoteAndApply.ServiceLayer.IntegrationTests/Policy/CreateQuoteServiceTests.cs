using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Tests.Shared.Builders;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy
{
    [TestFixture]
    public class CreateQuoteServiceTests : BaseServiceLayerTest
    {
        [SetUp]
        public void Setup()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();
        }

        [Test]
        public void CreateQuote_WithCleanOccupation_SavesCorrectUnderwritingStatusToDb()
        {
            //Act
            var quote = CreateQuoteWithDefaults();

            //Assert
            var riskService = GetServiceInstance<IPolicyWithRisksService>();
            var planService = GetServiceInstance<IPlanService>();
            var coverService = GetServiceInstance<ICoverService>();

            var policyWithRisks = riskService.GetFrom(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            var plans = planService.GetPlansForRisk(risk.Risk.Id);
            var lifeCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "DTH").Id);
            var ciCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TRS").Id);
            var tpdCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TPS").Id);
            var ipCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "IP").Id);

            Assert.That(lifeCovers.All(c => c.UnderwritingStatus == UnderwritingStatus.Incomplete));
            Assert.That(ciCovers.All(c => c.UnderwritingStatus == UnderwritingStatus.Incomplete));
            Assert.That(tpdCovers.All(c => c.UnderwritingStatus == UnderwritingStatus.Incomplete));
            Assert.That(ipCovers.All(c => c.UnderwritingStatus == UnderwritingStatus.Incomplete));
        }


        [Test]
        public void CreateQuote_WithPilotOccupation_SavesCorrectUnderwritingStatusToDb()
        {
            //Act
            var quote = CreateQuote(new CreateQuoteBuilder()
                .AsCommercialPilot()
                .Build());

            //Assert
            var riskService = GetServiceInstance<IPolicyWithRisksService>();
            var planService = GetServiceInstance<IPlanService>();
            var coverService = GetServiceInstance<ICoverService>();

            var policyWithRisks = riskService.GetFrom(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            var plans = planService.GetPlansForRisk(risk.Risk.Id);
            var lifeCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "DTH").Id);
            var ciCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TRS").Id);
            var tpdCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TPS").Id);
            var ipCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "IP").Id);

            Assert.That(lifeCovers.All(c => c.UnderwritingStatus == UnderwritingStatus.Incomplete));
            Assert.That(ciCovers.All(c => c.UnderwritingStatus == UnderwritingStatus.Incomplete));
            Assert.That(tpdCovers.All(c => c.UnderwritingStatus == UnderwritingStatus.Decline));
            Assert.That(ipCovers.All(c => c.UnderwritingStatus == UnderwritingStatus.Decline));
        }

        [Test]
        public void CreateQuote_LetMePlayJourney_WithCleanOccupation_HasCorrectPlansAndCoversSelected()
        {

            //Act
            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            //Assert
            var riskService = GetServiceInstance<IPolicyWithRisksService>();
            var planService = GetServiceInstance<IPlanService>();
            var coverService = GetServiceInstance<ICoverService>();

            var policyWithRisks = riskService.GetFrom(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            var plans = planService.GetPlansForRisk(risk.Risk.Id);
            var lifeCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "DTH").Id);
            var ciCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TRS").Id);
            var tpdCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TPS").Id);
            var ipCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "IP").Id);

            Assert.That(plans.Single(p => p.Code == "DTH").Selected, Is.True, "Life cover should be selected");
            Assert.That(lifeCovers.All(c => c.Selected), "All life covers should be selected");
            //TODO: still need to check rider covers on LIFE

            Assert.That(plans.Single(p => p.Code == "TRS").Selected, Is.False, "CI cover should not be selected");
            Assert.That(ciCovers.All(c => c.Selected), "All CI covers should be selected");

            Assert.That(plans.Single(p => p.Code == "TPS").Selected, Is.False, "TPD cover should not be selected");
            Assert.That(tpdCovers.All(c => c.Selected), "All TPD covers should be selected");

            Assert.That(plans.Single(p => p.Code == "IP").Selected, Is.True, "IP cover should be selected");
            Assert.That(ipCovers.All(c => c.Selected), "All IP covers should be selected");
        }

        [Test]
        public void CreateQuote_LetMePlayJourney_WithPilotOccupation_TurnsOffUnavailablePlansAndCovers()
        {
            //Arrange
            MockProductErrorMessageService.Setup(call => call.GetPlanHasNoValidOptionsErrorMessage())
                .Returns("This plan has no valid options");
            MockPlanErrorMessageService.Setup(call => call.GetSelectedCoverUndwritingDeclinedMessage())
                .Returns("This cover is declined");

            //Act
            var quote = CreateQuote(new CreateQuoteBuilder()
                .AsCommercialPilot()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            //Assert
            var riskService = GetServiceInstance<IPolicyWithRisksService>();
            var planService = GetServiceInstance<IPlanService>();
            var coverService = GetServiceInstance<ICoverService>();

            var policyWithRisks = riskService.GetFrom(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            var plans = planService.GetPlansForRisk(risk.Risk.Id);
            var lifeCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "DTH").Id);
            var ciCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TRS").Id);
            var tpdCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TPS").Id);
            var ipCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "IP").Id);

            Assert.That(plans.Single(p => p.Code == "DTH").Selected, Is.True, "Life cover should be selected");
            Assert.That(lifeCovers.All(c => c.Selected), "All life covers should be selected");
            //TODO: still need to check rider covers on LIFE

            Assert.That(plans.Single(p => p.Code == "TRS").Selected, Is.False, "CI cover should not be selected");
            Assert.That(ciCovers.All(c => c.Selected), "All CI covers should be selected");

            Assert.That(plans.Single(p => p.Code == "TPS").Selected, Is.False, "TPD cover should not be selected");
            Assert.That(tpdCovers.All(c => !c.Selected), "No TPD covers should be selected");

            Assert.That(plans.Single(p => p.Code == "IP").Selected, Is.False, "IP cover should not be selected");
            Assert.That(ipCovers.All(c => !c.Selected), "No IP covers should be selected");
        }

        [Test]
        public void CreateQuote_LetMePlayJourney_WithPersonAged60_TurnsOffUnavailablePlansAndCovers()
        {
            //Arrange
            MockProductErrorMessageService.Setup(call => call.GetMaximumAgeErrorMessage(61))
                .Returns("You're too old yo");

            //Act
            var quote = CreateQuote(new CreateQuoteBuilder()
                .AsAge(60)
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            //Assert
            var riskService = GetServiceInstance<IPolicyWithRisksService>();
            var planService = GetServiceInstance<IPlanService>();
            var coverService = GetServiceInstance<ICoverService>();

            var policyWithRisks = riskService.GetFrom(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            var plans = planService.GetPlansForRisk(risk.Risk.Id);
            var lifeCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "DTH").Id);
            var ciCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TRS").Id);
            var tpdCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TPS").Id);
            var ipCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "IP").Id);

            Assert.That(plans.Single(p => p.Code == "DTH").Selected, Is.True, "Life cover should be selected");
            Assert.That(lifeCovers.All(c => c.Selected), "All life covers should be selected");
            //TODO: still need to check rider covers on LIFE

            Assert.That(plans.Single(p => p.Code == "TRS").Selected, Is.False, "CI cover should not be selected");
            Assert.That(ciCovers.All(c => c.Selected), "All CI covers should be selected");

            Assert.That(plans.Single(p => p.Code == "TPS").Selected, Is.False, "TPD cover should not be selected");
            Assert.That(tpdCovers.All(c => c.Selected), "All TPD covers should be selected");

            Assert.That(plans.Single(p => p.Code == "IP").Selected, Is.False, "IP cover should not be selected");
            Assert.That(ipCovers.All(c => c.Selected), "All IP covers should be selected");
        }


        [Test]
        public void CreateQuote_LetMePlayJourney_Under60WithNoOccupation_ThenIpIsSelected()
        {

            //Act
            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithNoOccupation()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            //Assert
            var riskService = GetServiceInstance<IPolicyWithRisksService>();
            var planService = GetServiceInstance<IPlanService>();
            var coverService = GetServiceInstance<ICoverService>();

            var policyWithRisks = riskService.GetFrom(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            var plans = planService.GetPlansForRisk(risk.Risk.Id);
            var lifeCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "DTH").Id);
            var ciCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TRS").Id);
            var tpdCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TPS").Id);
            var ipCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "IP").Id);

            Assert.That(plans.Single(p => p.Code == "DTH").Selected, Is.True, "Life cover should be selected");
            Assert.That(lifeCovers.All(c => c.Selected), "All life covers should be selected");
            //TODO: still need to check rider covers on LIFE

            Assert.That(plans.Single(p => p.Code == "TRS").Selected, Is.False, "CI cover should not be selected");
            Assert.That(ciCovers.All(c => c.Selected), "All CI covers should be selected");

            Assert.That(plans.Single(p => p.Code == "TPS").Selected, Is.False, "TPD cover should not be selected");
            Assert.That(tpdCovers.All(c => c.Selected), "TPD covers should be selected");

            Assert.That(plans.Single(p => p.Code == "IP").Selected, Is.True, "IP cover should be selected");
            Assert.That(ipCovers.All(c => c.Selected), "All IP covers should be selected");
        }

        [Test]
        public void CreateQuote_LetMePlayJourney_Age60WithNoOccupation_ThenIpNotSelected()
        {
            //Arrange
            MockProductErrorMessageService.Setup(call => call.GetMaximumAgeErrorMessage(60))
                .Returns("You're too old yo");

            //Act
            var quote = CreateQuote(new CreateQuoteBuilder()
                .AsAge(60)
                .WithNoOccupation()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            //Assert
            var riskService = GetServiceInstance<IPolicyWithRisksService>();
            var planService = GetServiceInstance<IPlanService>();
            var coverService = GetServiceInstance<ICoverService>();

            var policyWithRisks = riskService.GetFrom(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            var plans = planService.GetPlansForRisk(risk.Risk.Id);
            var lifeCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "DTH").Id);
            var ciCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TRS").Id);
            var tpdCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "TPS").Id);
            var ipCovers = coverService.GetCoversForPlan(plans.Single(p => p.Code == "IP").Id);

            Assert.That(plans.Single(p => p.Code == "DTH").Selected, Is.True, "Life cover should be selected");
            Assert.That(lifeCovers.All(c => c.Selected), "All life covers should be selected");
            //TODO: still need to check rider covers on LIFE

            Assert.That(plans.Single(p => p.Code == "TRS").Selected, Is.False, "CI cover should not be selected");
            Assert.That(ciCovers.All(c => c.Selected), "All CI covers should be selected");

            Assert.That(plans.Single(p => p.Code == "TPS").Selected, Is.False, "TPD cover should not be selected");
            Assert.That(tpdCovers.All(c => c.Selected), "TPD covers should be selected");

            Assert.That(plans.Single(p => p.Code == "IP").Selected, Is.False, "IP cover should not be selected");
            Assert.That(ipCovers.All(c => c.Selected), "All IP covers should be selected");
        }

    }
}

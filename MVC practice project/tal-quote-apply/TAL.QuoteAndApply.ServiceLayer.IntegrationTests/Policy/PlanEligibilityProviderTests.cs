using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.Tests.Shared.Builders;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy
{
    [TestFixture]
    public class PlanEligibilityProviderTests : BaseServiceLayerTest
    {
        [SetUp]
        public void Setup()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //Setup default mocks
            MockProductErrorMessageService.Setup(call => call.GetPlanHasNoValidOptionsErrorMessage())
                .Returns("This plan has no valid options");

            MockPlanErrorMessageService.Setup(call => call.GetSelectedCoverUndwritingDeclinedMessage())
                .Returns("This cover is declined");

            MockProductErrorMessageService.Setup(call => call.GetMaximumAgeErrorMessage(It.IsAny<int>()))
                .Returns("You're too old yo");
        }

        [Test]
        public void GetPlanEligibilitiesFor_DefaultQuote_AllPlansAndCoversAreEligible()
        {

            var eligibility = SetupAndGetEligibilitiesFor(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn));

            AssertPlanAndCoverEligibility(eligibility, "DTH", true); //Life plan and covers should be eligible
            AssertPlanAndCoverEligibility(eligibility, "TRS", true); //CI plan and covers should be eligible
            AssertPlanAndCoverEligibility(eligibility, "TPS", true); //TPD plan and covers should be eligible
            AssertPlanAndCoverEligibility(eligibility, "IP", true); //IP plan and covers should be eligible
        }

        [Test]
        public void GetPlanEligibilitiesFor_PilotOccupation_TPDAndIPIsNotEligibile()
        {

            var eligibility = SetupAndGetEligibilitiesFor(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .AsCommercialPilot());

            AssertPlanAndCoverEligibility(eligibility, "DTH", true); //Life plan and covers should be eligible
            AssertPlanAndCoverEligibility(eligibility, "TRS", true); //CI plan and covers should be eligible
            AssertPlanAndCoverEligibility(eligibility, "TPS", false); //TPD plan and covers should not be eligible
            AssertPlanAndCoverEligibility(eligibility, "IP", false); //IP plan and covers should not be eligible
        }

        [Test]
        public void GetPlanEligibilitiesFor_NoOccupationAndUnder60_AllPlansAndCoversEligible()
        {

            var eligibility = SetupAndGetEligibilitiesFor(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .WithNoOccupation());

            AssertPlanAndCoverEligibility(eligibility, "DTH", true); //Life plan and covers should be eligible
            AssertPlanAndCoverEligibility(eligibility, "TRS", true); //CI plan and covers should be eligible
            AssertPlanAndCoverEligibility(eligibility, "TPS", true); //TPD plan and covers should be eligible
            AssertPlanAndCoverEligibility(eligibility, "IP", true); //IP plan and covers should be eligible
        }

        [Test]
        public void GetPlanEligibilitiesFor_HasSafeOccupationAndOver60_OnlyLifeIsEligible()
        {

            var eligibility = SetupAndGetEligibilitiesFor(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .AsAge(60));

            AssertPlanAndCoverEligibility(eligibility, "DTH", true); //Life plan and covers should be eligible
            AssertPlanAndCoverEligibility(eligibility, "TRS", false, true); //CI plan is not eligible but covers are
            AssertPlanAndCoverEligibility(eligibility, "TPS", false, true); //TPD plan is not eligible but covers are
            AssertPlanAndCoverEligibility(eligibility, "IP", false, true); //IP plan is not eligible but covers are
        }

        [Test]
        public void GetPlanEligibilitiesFor_PersonOver60WithNoOccupation_OnlyLifeIsEligible()
        {

            var eligibility = SetupAndGetEligibilitiesFor(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .AsAge(60)
                .WithNoOccupation());

            AssertPlanAndCoverEligibility(eligibility, "DTH", true); //Life plan and covers should be eligible
            AssertPlanAndCoverEligibility(eligibility, "TRS", false, true); //CI plan is not eligible but covers are
            AssertPlanAndCoverEligibility(eligibility, "TPS", false, true); //TPD plan is not eligible but covers are
            AssertPlanAndCoverEligibility(eligibility, "IP", false, true); //IP plan is not eligible but covers are
        }

        private List<PlanEligibilityResult> SetupAndGetEligibilitiesFor(CreateQuoteBuilder quoteBuilder)
        {
            var quote = CreateQuote(quoteBuilder.Build());

            var policyWithRisks = GetServiceInstance<IPolicyWithRisksService>().GetFrom(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            var eligibility = GetServiceInstance<IPlanEligibilityProvider>().GetPlanEligibilitiesFor(risk.Risk.Id);
            return eligibility.ToList();
        }

        private void AssertPlanAndCoverEligibility(IList<PlanEligibilityResult> eligibilities, string planCode,
            bool planEligible, bool? coverEligible = null)
        {

            //If no coverEligibile value then set to be planEligible value
            if (coverEligible == null)
            {
                coverEligible = planEligible;
            }

            var eligibility = eligibilities.Single(e => e.PlanCode == planCode);
            Assert.That(eligibility.EligibleForPlan, Is.EqualTo(planEligible));
            Assert.That(eligibility.CoverEligibilityResults.All(c => c.EligibleForCover == coverEligible));
        }
    }
}

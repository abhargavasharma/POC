using System.Collections.Generic;
using System.Linq;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests
{
    [TestFixture]
    public class RetrieveApiTests : BaseTestClass<RetrieveClient>
    {
        public RetrieveApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }


        [Test]
        public async Task RetrieveQuote_WhenNoFieldsInRequest_ReturnsErrorModelState_Async()
        {
            //Arrange
            var request = new RetrieveQuoteRequest { };

            //Act
            var response = await Client.RetrieveQuoteAsync<Dictionary<string, IEnumerable<string>>>(request);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(
                response.First(x => x.Key.Equals("retrieveQuoteRequest.quoteReference")).Value.First(),
                Is.EqualTo("Reference number is required"));
            Assert.That(
                response.First(x => x.Key.Equals("retrieveQuoteRequest.password")).Value.First(),
                Is.EqualTo("Password is required"));
        }


        [Test]
        public async Task RetrieveQuote_WhenSavedAndRetrievedWithCorrectPassword_ReturnsSuccessfulRedirectAction_Async()
        {
            //Arrange
            const string password = "AAbb66^^";
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var originalQuoteReference = QuoteSessionContext.QuoteSession.QuoteReference;

            await SavePolicyWithDefaultsAsync(risk.Id, password);

            QuoteSessionContext.Clear(); //Simulate logout

            //Act
            var request = new RetrieveQuoteRequest
            {
                Password = password,
                QuoteReference = originalQuoteReference
            };

            var response = await Client.RetrieveQuoteAsync<RedirectToResponse>(request);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.RedirectTo, Is.EqualTo("/SelectCover"));
            Assert.That(QuoteSessionContext.QuoteSession.QuoteReference, Is.EqualTo(originalQuoteReference)); //Check that session was set to correct quote reference
            Assert.That(QuoteSessionContext.QuoteSession.SessionData.CallBackRequested, Is.EqualTo(false));
            Assert.That(QuoteHasInteraction(originalQuoteReference, InteractionType.Retrieved_By_Customer));
        }

        [Test]
        public async Task RetrieveQuote_WhenSavedUntilReviewAndRetrieved_ShouldRedirectToReviewAndHaveReferAsRiskMarketingStatus()
        {
            //Arrange
            const string password = "AAbb66^^";
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var originalQuoteReference = QuoteSessionContext.QuoteSession.QuoteReference;
            await SavePolicyWithDefaultsAsync(risk.Id, password);

            var reviewResonse = await Client.GetReviewForRiskAsync(risk.Id);
            var lifePlan = reviewResonse.Cover.Plans.Single(p => p.PlanCode == "DTH");
            var tpdPlan = reviewResonse.Cover.Plans.Single(p => p.PlanCode == "TPS");
            var riPlan = reviewResonse.Cover.Plans.Single(p => p.PlanCode == "TRS");
            var ipPlan = reviewResonse.Cover.Plans.Single(p => p.PlanCode == "IP");

            //Only Select Life
            await UpdatePlans<UpdatePlanResponse>(risk.Id, lifePlan, new[] { "DTHAC", "DTHASC", "DTHIC" }, new[] { "DTH", "TRS" });          

            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            //Setup so customer will be accept on life but refer on RI
            await customerUnderwriting.AnswerCleanFullUnderwriting();
            await customerUnderwriting.AnswerToGetFibromyalgiaOutcome(); //This will give Refer on Recovery Insurance
            FinaliseCustomerUnderwriting(risk.Id);

            //Act
            var request = new RetrieveQuoteRequest
            {
                Password = password,
                QuoteReference = originalQuoteReference
            };

            var response = await Client.RetrieveQuoteAsync<RedirectToResponse>(request);

            //Check that we are redirecting to Review page (As referred)
            Assert.That(response, Is.Not.Null);
            Assert.That(response.RedirectTo, Is.EqualTo("/Review"));

            //Assert
            Assert.That(RiskMarketingStatusIsEqualTo(MarketingStatus.Refer, risk.Id), Is.True, "MarketingStatus incorrect on risk");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Lead, lifePlan.PlanId), Is.True, "MarketingStatus incorrect on plan " + lifePlan.PlanCode);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, tpdPlan.PlanId), Is.True, "MarketingStatus incorrect on plan " + tpdPlan.PlanCode);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Refer, riPlan.PlanId), Is.True, "MarketingStatus incorrect on plan " + riPlan.PlanCode);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, ipPlan.PlanId), Is.True, "MarketingStatus incorrect on plan " + ipPlan.PlanCode);

        }

        [Test]
        public async Task RetrieveQuote_WhenSavedAndRetrievedWithIncorrectPassword_ReturnsErrorModelState_Async()
        {
            //Arrange
            const string password = "AAbb66^^";
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var originalQuoteReference = QuoteSessionContext.QuoteSession.QuoteReference;

            await SavePolicyWithDefaultsAsync(risk.Id, password + "blah");

            QuoteSessionContext.Clear(); //Simulate logout

            //Act
            var request = new RetrieveQuoteRequest
            {
                Password = password,
                QuoteReference = originalQuoteReference
            };

            var response = await Client.RetrieveQuoteAsync<Dictionary<string, IEnumerable<string>>>(request);

            //Assert
            Assert.That(QuoteSessionContext.QuoteSession, Is.Null); //Make sure session was not set
            Assert.That(response, Is.Not.Null);
            Assert.That(
                response.First(x => x.Key.Equals("retrieval")).Value.First(),
                Is.EqualTo("Combination of reference number and password are invalid"));
            Assert.That(QuoteHasInteraction(originalQuoteReference, InteractionType.Retrieved_By_Customer), Is.False);
        }

        [Test]
        public async Task RetrieveQuote_WhenRetrievingNonExistingQuote_ReturnsErrorModelState_Async()
        {
            //Arrange
            var request = new RetrieveQuoteRequest
            {
                Password = "AAbb66^^",
                QuoteReference = "WillNeverExist"
            };

            //Act
            var response = await Client.RetrieveQuoteAsync<Dictionary<string, IEnumerable<string>>>(request);

            //Assert
            Assert.That(QuoteSessionContext.QuoteSession?.QuoteReference, Is.Null); //Make sure session was not set
            Assert.That(response, Is.Not.Null);
            Assert.That(
                response.First(x => x.Key.Equals("retrieval")).Value.First(),
                Is.EqualTo("Combination of reference number and password are invalid"));
        }

        [Test]
        public async Task RetrieveQuote_WhenQuoteNotSaved_ReturnsErrorModelState_Async()
        {
            //Arrange
            const string password = "AAbb66^^";
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var originalQuoteReference = QuoteSessionContext.QuoteSession.QuoteReference;

            QuoteSessionContext.Clear(); //Simulate logout

            //Act
            var request = new RetrieveQuoteRequest
            {
                Password = password,
                QuoteReference = originalQuoteReference
            };

            var response = await Client.RetrieveQuoteAsync<Dictionary<string, IEnumerable<string>>>(request);

            //Assert
            Assert.That(QuoteSessionContext.QuoteSession, Is.Null); //Make sure session was not set
            Assert.That(response, Is.Not.Null);
            Assert.That(
                response.First(x => x.Key.Equals("retrieval")).Value.First(),
                Is.EqualTo("Combination of reference number and password are invalid"));
            Assert.That(QuoteHasInteraction(originalQuoteReference, InteractionType.Retrieved_By_Customer), Is.False);
        }
    }
}

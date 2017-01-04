using System;
using System.Linq;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models;
using Task=System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    public class DashboardApiTests : BaseTestClass<DashboardClient>
    {
        public DashboardApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
            
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            
        }

        [Test]
        public async Task GetUnderwritingReferrals_AtLeastOneReferralReturned_Async()
        {
            AddUnderwriterPermission();

            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var planClient = GetSalesPortalPlanClient();

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await planClient.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new string[0],
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" }
            };

            await planClient.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            await GetSalesPortalReferralClient().CreateReferralAsync(policyWithRisks.Policy.QuoteReference);

            var result = await Client.GetReferralsAsync<ReferralsDetailsResponse>();

            Assert.That(result.Underwriters.Count(), Is.GreaterThan(0));
            Assert.That(result.Referrals.Count(), Is.GreaterThan(0));
            var returnedReferral =
                result.Referrals.First(x => x.QuoteReferenceNumber == policyWithRisks.Policy.QuoteReference);
            Assert.That(returnedReferral.State, Is.EqualTo(ReferralState.Unresolved));
            Assert.That(returnedReferral.Plans.Count(), Is.EqualTo(1));
            Assert.That(returnedReferral.Plans.First(), Is.EqualTo("DTH"));
        }
        
        [Test]
        public async Task GetUnderwritingReferrals_WithNoUnderwriterPermission_ReturnsBadRequest_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var planClient = GetSalesPortalPlanClient();

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await planClient.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new string[0],
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" }
            };

            await planClient.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            await GetSalesPortalReferralClient().CreateReferralAsync(policyWithRisks.Policy.QuoteReference);

            var referrals = await Client.GetReferralsAsync<UnauthorisedRoleResult>(throwOnFailure: false);
        }

        [Test]
        public async Task GetQuotes_AgentWithAtLeastOneQuote_AllPlansSelected_AndPipelineStatusInProgressPreUw_ReturnsCorrectQuote()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "TEST123CUSTREF"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));
          
            var policyClient = GetSalesPortalPolicyClient();

            var updatePolicyProgress = new PolicyProgressViewModel
            {
                Progress = PolicyProgress.InProgressPreUw.ToString()
            };

            await policyClient.UpdatePolicyProgressAsync<PolicyProgressViewModel>(response.QuoteReference, updatePolicyProgress);

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(response.QuoteReference);

            var interactionsService = Container.Resolve<IPolicyInteractionService>();

            interactionsService.PolicyAccessed(policyWithRisks.Policy.Id);

            var agentDashboardRequest = new AgentDashboardRequest()
            {
                PageNumber = 1,
                ClosedCantContact = false,
                ClosedNoSale = false,
                ClosedTriage = false,
                ClosedSale = false,
                InProgressPreUw = false,
                InProgressCantContact = false,
                InProgressUwReferral = false,
                InProgressRecommendation = false,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddMonths(-1),
                Brand = "TAL"
            };

            var result = await Client.GetQuotesAsync<AgentDashboardDetailsResponse>(agentDashboardRequest);

            Assert.That(result.Quotes.Count(), Is.GreaterThan(0));
            var returnQuote = result.Quotes.First(x => x.QuoteReference == response.QuoteReference);
            Assert.That(returnQuote.Progress, Is.EqualTo(updatePolicyProgress.Progress));
            Assert.That(returnQuote.LastTouchedByTS, Is.LessThan(DateTime.Now));
            Assert.That(returnQuote.Plans, Is.EqualTo("Life, Income"));
            Assert.That(returnQuote.PolicyId, Is.EqualTo(policyWithRisks.Policy.Id));
            Assert.That(returnQuote.Premium, Is.EqualTo(policyWithRisks.Policy.Premium));
            Assert.That(returnQuote.ProgressUpdateTs, Is.LessThan(DateTime.Now));
            Assert.That(returnQuote.QuoteReference, Is.EqualTo(policyWithRisks.Policy.QuoteReference));
            Assert.That(result.TotalRecords, Is.GreaterThan(0));
            Assert.That(returnQuote.Brand, Is.EqualTo("TAL"));
            Assert.That(returnQuote.ExternalCustomerReference, Is.EqualTo("TEST123CUSTREF"));
        }

        [Test]
        public async Task GetQuotes_AgentWithAtLeastOneQuote_AllPlansSelected_AndPipelineStatusInProgressPreUwForExternalCustomers_ReturnsCorrectQuote()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "NEW123CUSTREF"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var policyClient = GetSalesPortalPolicyClient();

            var updatePolicyProgress = new PolicyProgressViewModel
            {
                Progress = PolicyProgress.InProgressPreUw.ToString()
            };

            await policyClient.UpdatePolicyProgressAsync<PolicyProgressViewModel>(response.QuoteReference, updatePolicyProgress);

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(response.QuoteReference);

            var interactionsService = Container.Resolve<IPolicyInteractionService>();

            interactionsService.PolicyAccessed(policyWithRisks.Policy.Id);

            var agentDashboardRequest = new AgentDashboardRequest()
            {
                PageNumber = 1,
                ClosedCantContact = false,
                ClosedNoSale = false,
                ClosedTriage = false,
                ClosedSale = false,
                InProgressPreUw = false,
                InProgressCantContact = false,
                InProgressUwReferral = false,
                InProgressRecommendation = false,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddMonths(-1),
                Brand = "TAL"
            };

            var result = await Client.GetQuotesAsync<AgentDashboardDetailsResponse>(agentDashboardRequest);

            Assert.That(result.Quotes.Count(), Is.GreaterThan(0));
            var returnQuote = result.Quotes.First(x => x.QuoteReference == response.QuoteReference);
            Assert.That(returnQuote.Progress, Is.EqualTo(updatePolicyProgress.Progress));
            Assert.That(returnQuote.LastTouchedByTS, Is.LessThan(DateTime.Now));
            Assert.That(returnQuote.Plans, Is.EqualTo("Life, Income"));
            Assert.That(returnQuote.PolicyId, Is.EqualTo(policyWithRisks.Policy.Id));
            Assert.That(returnQuote.Premium, Is.EqualTo(policyWithRisks.Policy.Premium));
            Assert.That(returnQuote.ProgressUpdateTs, Is.LessThan(DateTime.Now));
            Assert.That(returnQuote.QuoteReference, Is.EqualTo(policyWithRisks.Policy.QuoteReference));
            Assert.That(result.TotalRecords, Is.GreaterThan(0));
            Assert.That(returnQuote.Brand, Is.EqualTo("TAL"));
            Assert.That(returnQuote.ExternalCustomerReference, Is.EqualTo("NEW123CUSTREF"));
        }
    }
}
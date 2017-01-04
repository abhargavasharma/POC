using System;
using System.Linq;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Referral;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    [TestFixture]
    public class ReferralApiTests : BaseTestClass<ReferralClient>
    {
        public ReferralApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [Test]
        public async Task CreateReferral_ReferralAlreadyInProgress_ReturnsError_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-59).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var createReferralService = Container.Resolve<ICreateReferralService>();
            createReferralService.CreateReferralFor(policyWithRisks.Policy.QuoteReference);

            var errors = await Client.CreateReferralReturnErrorAsync(policyWithRisks.Policy.QuoteReference);

            Assert.That(errors, Is.Not.Null);
            Assert.That(errors.ContainsKey("referral"), Is.True);
            Assert.That(errors.First(x => x.Key.Equals("referral")).Value.First(),
                Is.EqualTo("An in progress referral already exists for this policy"));
        }

        [Test]
        public async Task CreateReferral_NoExistingInProgressReferral_ReferralCreated_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-59).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            await Client.CreateReferralAsync(policyWithRisks.Policy.QuoteReference);

            var referralService = Container.Resolve<IReferralService>();
            var referral = referralService.GetInprogressReferralForPolicy(policyWithRisks.Policy.Id);

            Assert.That(referral, Is.Not.Null);
            Assert.That(referral.CompletedBy, Is.Null);
            Assert.That(referral.CompletedTS, Is.Null);

            var policyInteractionSvc = Container.Resolve<IPolicyInteractionService>();
            var allInteractions = policyInteractionSvc.GetInteractions(PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(policyWithRisks.Policy.QuoteReference));
            var referredToUnderwriterInteraction = allInteractions.Interactions.FirstOrDefault(x => x.InteractionType == InteractionType.Referred_To_Underwriter);

            Assert.That(referredToUnderwriterInteraction, Is.Not.Null);
        }

        [Test]
        public async Task CompleteReferral_InProgressReferral_MarkedAsCompleted_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-59).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var createReferralService = Container.Resolve<ICreateReferralService>();
            createReferralService.CreateReferralFor(policyWithRisks.Policy.QuoteReference);

            var referralRepo = Container.Resolve<IReferralDtoRepository>();
            var referral = referralRepo.GetInprogressReferralForPolicy(policyWithRisks.Policy.Id);

            await Client.CompleteReferralAsync(policyWithRisks.Policy.QuoteReference);

            referral = referralRepo.GetForPolicy(referral.PolicyId);

            Assert.That(referral.IsCompleted, Is.True);
            Assert.That(referral.CompletedBy, Is.EqualTo("IntegrationTests"));

            var policyInteractionSvc = Container.Resolve<IPolicyInteractionService>();
            var allInteractions = policyInteractionSvc.GetInteractions(PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(policyWithRisks.Policy.QuoteReference));
            var referralCompleteInteraction = allInteractions.Interactions.FirstOrDefault(x => x.InteractionType == InteractionType.Referral_Completed_By_Underwriter);

            Assert.That(referralCompleteInteraction, Is.Not.Null);
        }

        [Test]
        public async Task ReferralWorkflowTests_CreateAndCompleteMultipleTimes_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-59).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var referralRepo = Container.Resolve<IReferralDtoRepository>();
            var attempts = 3;

            //create and complete the referral multiple times to make sure DE5971 is fixed
            for (var i = 0; i <= attempts; i++)
            {
                await Client.CreateReferralAsync(policyWithRisks.Policy.QuoteReference);

                var referral = referralRepo.GetInprogressReferralForPolicy(policyWithRisks.Policy.Id);

                Assert.That(referral, Is.Not.Null);

                await Client.CompleteReferralAsync(policyWithRisks.Policy.QuoteReference);

                referral = referralRepo.GetForPolicy(referral.PolicyId);

                Assert.That(referral.IsCompleted, Is.True);
                Assert.That(referral.CompletedBy, Is.EqualTo("IntegrationTests"));
            }
        }

        [Test]
        public async Task AssignReferral_InProgressReferral_MarkedAsInProgress_Async()
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

            var request = new AssignReferralRequest();

            request.Name = "Eddie Vedder";

            var result = await Client.AssignReferralAsync<ReferralDetailsResponse>(policyWithRisks.Policy.QuoteReference, request);

            Assert.That(result.State, Is.EqualTo(ReferralState.InProgress));
            Assert.That(result.Plans.Count(), Is.EqualTo(1));
            Assert.That(result.Plans.First(), Is.EqualTo("DTH"));
        }

        [Test]
        public async Task AssignReferral_UnassignReferralBySelectingNoUnderwriter_MarkedAsUnresolved_Async()
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

            var request = new AssignReferralRequest();

            request.Name = null;

            var result = await Client.AssignReferralAsync<ReferralDetailsResponse>(policyWithRisks.Policy.QuoteReference, request);

            Assert.That(result.State, Is.EqualTo(ReferralState.Unresolved));
            Assert.That(result.Plans.Count(), Is.EqualTo(1));
            Assert.That(result.Plans.First(), Is.EqualTo("DTH"));
        }
    }
}
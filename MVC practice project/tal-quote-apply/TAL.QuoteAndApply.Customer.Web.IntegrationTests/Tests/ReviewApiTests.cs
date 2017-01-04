using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.Customer.Web.Extensions;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests
{
    [TestFixture]
    public class ReviewApiTests : BaseTestClass<ReviewClient>
    {
        public ReviewApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [Test]
        public async Task GetReview_ForRisk_ReturnsPopulatedReviewModel_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();

            //Act
            var response = await Client.GetReviewForRiskAsync(risk.Id);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ReviewWorkflowStatus, Is.EqualTo(ReviewWorkflowStatus.Refer));
            Assert.That(response.Cover, Is.Not.Null);
        }


        [Test]
        public async Task ValidateReview_UnderwritingRefer_InteractionWrittenAndReturnsCorrectRedirectUrl_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();

            //Act
            var response = await Client.ValidateReviewForRiskAsync(risk.Id);

            //Assert
            Assert.That(response, Is.Not.Null);

            var policyInteractionSvc = Container.Resolve<IPolicyInteractionService>();
            var allInteractions = policyInteractionSvc.GetInteractions(PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(QuoteSessionContext.QuoteSession.QuoteReference));
            var referredToUnderwriterInteraction = allInteractions.Interactions.FirstOrDefault(x => x.InteractionType == InteractionType.Customer_Submit_Application_Referred);
            
            //interaction
            Assert.That(referredToUnderwriterInteraction, Is.Not.Null);
            
            //policy save status
            var policySvc = Container.Resolve<IPolicyService>();
            var policy = policySvc.GetByQuoteReferenceNumber(QuoteSessionContext.QuoteSession.QuoteReference);
            Assert.That(policy.SaveStatus, Is.EqualTo(PolicySaveStatus.LockedOutDueToRefer));

            //redirect url
            Assert.That(response.RedirectTo, Is.EqualTo("/Submission"));
        }

        [Test]
        public async Task UpdatePlan_WhenSwitchesFromAcceptToReferState_ReferStateIsSetCorrectly_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();

            var reviewResonse = await Client.GetReviewForRiskAsync(risk.Id);
            var lifePlan = reviewResonse.Cover.Plans.Single(p => p.PlanCode == "DTH");            

            //Only Select Life
            await UpdatePlans<UpdatePlanResponse>(risk.Id, lifePlan, new [] {"DTHAC", "DTHASC", "DTHIC"}, new [] {"DTH"});

            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            //Setup so customer will be accept on life but refer on RI
            await customerUnderwriting.AnswerCleanFullUnderwriting();
            await customerUnderwriting.AnswerToGetFibromyalgiaOutcome(); //This will give Refer on Recovery Insurance
            FinaliseCustomerUnderwriting(risk.Id);

            //Check that review page gives correct workflow status, should be in accept at this point because RI not selected
            var response = await Client.GetReviewForRiskAsync(risk.Id);
            Assert.That(response.ReviewWorkflowStatus, Is.EqualTo(ReviewWorkflowStatus.Accept));
            var dbOriginalRiskInterviewStatus = Container.Resolve<IRiskDtoRepository>().GetRisk(risk.Id).InterviewStatus;
            Assert.That(dbOriginalRiskInterviewStatus, Is.EqualTo(InterviewStatus.Complete)); //Also confirm risk table is set correctly


            //Turn on Recovery Insurance (so Life and RI will be selected)
            var recoveryInsurancePlan = response.Cover.Plans.Single(p => p.PlanCode == "TRS");
            var turnOnRiResponse = await UpdatePlans<PolicyReviewResponse>(risk.Id, recoveryInsurancePlan, new[] {"TRSCC", "TRSSIC", "TRSSIN"},
                    new[] {"DTH", "TRS"}, true);

            //Check that we're now in Referred state
            Assert.That(turnOnRiResponse.ReviewWorkflowStatus, Is.EqualTo(ReviewWorkflowStatus.Refer));
            var dbUpdatedRiskInterviewStatus = Container.Resolve<IRiskDtoRepository>().GetRisk(risk.Id).InterviewStatus;
            Assert.That(dbUpdatedRiskInterviewStatus, Is.EqualTo(InterviewStatus.Referred)); //Also confirm risk table is set correctly

            var plansResponse = await GetPlansForRisk(risk.Id);
            var plans = plansResponse.ToList();

            //This tests the second condition in marketing status rule spreadsheet
            Assert.That(RiskMarketingStatusIsEqualTo(MarketingStatus.Refer, risks.First().Id), Is.True);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Lead, plans[0].PlanId), Is.True);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[1].PlanId), Is.True);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Refer, plans[2].PlanId), Is.True);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[3].PlanId), Is.True);

            var validateResponse = await Client.ValidateReviewForRiskAsync(risk.Id);
            Assert.That(validateResponse.RedirectTo, Is.EqualTo("/Submission"));
        }

        [Test]
        public async Task SetPremiumType_ToLevel_ReturnsMuchSuccessfulResponse_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();

            //Act
            var response = await Client.SetPremiumTypeAsync<PolicyReviewResponse>(risk.Id,
                new PremiumTypeUpdateRequest {PremiumType = PremiumType.Level});

            //Assert
            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task SetPremiumType_ToLevelPremiumTypeButTooOld_ReturnsModelStateErrors_Async()
        {
            //Arrange
            var risks = await BasicInfoLoginAsAsync(new BasicInfoViewModel
            {
                DateOfBirth = DateTime.Now.AddYears(-61).ToFriendlyString(),
                AnnualIncome = 1,
                Gender = 'M',
                IsSmoker = true,
                IndustryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing,
                OccupationCode = UnderwritingHelper.OccupationCode_AccountExecutive,
                Postcode = "1000"
            });

            var risk = risks.First();

            //Act
            var response = await Client.SetPremiumTypeAsync<Dictionary<string, IEnumerable<string>>>(risk.Id,
                new PremiumTypeUpdateRequest { PremiumType = PremiumType.Level });

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("premiumType"), Is.True);
        }


        [Test]
        public async Task GetReview_WhenHasForcedLoading_ReturnsLoading_Async()
        {
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            await customerUnderwriting.AnswerToGetKidneyStonesLoadingAsync();

            var response = await Client.GetReviewForRiskAsync(risk.Id);

            Assert.That(response.SharedLoadings.Any());

            var loading = response.SharedLoadings.First();
            Assert.That(loading.Name, Is.EqualTo("Kidney Stone"));
            Assert.That(loading.PremiumDiff, Is.GreaterThan(0));
            Assert.That(loading.ApplicablePlanCodes.Any());
        }

        [Test]
        public async Task GetReview_WhenHasForcedLoadingDueToOccupation_ReturnsLoading_Async()
        {
            //dogman occupation
            var risks = await BasicInfoDefaultLoginAsync(occupationCode: "xw", industryCode: "2uc");
            var risk = risks.First();
            
            var response = await Client.GetReviewForRiskAsync(risk.Id);

            Assert.That(response.SharedLoadings.Any());

            var loading = response.SharedLoadings.First();
            Assert.That(loading.Name, Is.EqualTo("Occupation"));
            Assert.That(loading.PremiumDiff, Is.GreaterThan(0));
            Assert.That(loading.ApplicablePlanCodes.Any());
        }

        [Test]
        public async Task GetReview_WhenUnderwritingAllCoversUnavailable_OutcomeIsReferred_Async()
        {
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            await customerUnderwriting.MakeUnderwritingGlobalDecline();

            var response = await Client.GetReviewForRiskAsync(risk.Id);

            Assert.That(response.ReviewWorkflowStatus, Is.EqualTo(ReviewWorkflowStatus.Refer));
        }

        [Test]
        public async Task GetReview_ForRiskWithChoiceToChangeLoadingToExclusion_ReturnsCorrectChoice_Async()
        {
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            var question = await customerUnderwriting.GetAsthmaExclusionLoadingQuestionAsync();
            await question.AnswerByTextAsync("Include Asthma");

            var response = await Client.GetReviewForRiskAsync(risk.Id);

            Assert.That(response.QuestionChoices.Count(), Is.EqualTo(1));

            var questionChoice = response.QuestionChoices.First();
            Assert.That(questionChoice.CurrentChoiceType, Is.EqualTo(UnderwritingQuestionChoiceType.Loading));
            Assert.That(questionChoice.Name, Is.EqualTo("Asthma"));
        }

        [Test]
        public async Task GetReview_ForRiskWithChoiceToChangeExclusionToLoading_ReturnsCorrectChoice_Async()
        {
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            var question = await customerUnderwriting.GetAsthmaExclusionLoadingQuestionAsync();
            await question.AnswerByTextAsync("Exclude Asthma");

            var response = await Client.GetReviewForRiskAsync(risk.Id);

            Assert.That(response.QuestionChoices.Count(), Is.EqualTo(1));

            var questionChoice = response.QuestionChoices.First();
            Assert.That(questionChoice.CurrentChoiceType, Is.EqualTo(UnderwritingQuestionChoiceType.Exclusion));
            Assert.That(questionChoice.Name, Is.EqualTo("Asthma"));
        }


        [Test]
        public async Task GetReview_ForRiskWithChoiceToChangeLoadingToExclusion_ChangeToExcusion_NewPremiumValuesReturned_Async()
        {
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            var question = await customerUnderwriting.GetAsthmaExclusionLoadingQuestionAsync();
            await question.AnswerByTextAsync("Include Asthma");
            FinaliseCustomerUnderwriting(risk.Id);

            var response = await Client.GetReviewForRiskAsync(risk.Id);

            var originalPremium = response.Cover.TotalPremium;
            var questionChoice = response.QuestionChoices.First();
            var originalLoadingAmount = questionChoice.PremiumDiff;

            var updateResponse = await Client.UpdateChoiceQuestionsAsync(risk.Id,
                new UpdateQuestionRequest
                {
                    QuestionId = questionChoice.QuestionId,
                    SelectedAnswers =
                        new List<UpdateQuestionAnswerRequest>
                        {
                            new UpdateQuestionAnswerRequest {Id = questionChoice.AnswerId}
                        }
                });

            Assert.That(updateResponse.Cover.TotalPremium, Is.EqualTo(originalPremium - originalLoadingAmount));      
        }

        [Test]
        public async Task GetReview_ForRiskWithExclusions_ReturnsExclusions_Async()
        {
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            var question = await customerUnderwriting.GetHangGlidingExclusionLoadingChoiceQuestionAsync();
            await question.AnswerByTextAsync("Exclude hang gliding activities");

            FinaliseCustomerUnderwriting(risk.Id);

            var response = await Client.GetReviewForRiskAsync(risk.Id);

            var plans = response.Cover.Plans;
            AssertCoverOnPlanContainsExclusion(plans, "DTH", "DTHASC", "Hang Gliding activities (Adventure Sports Cover)", true);
            AssertCoverOnPlanContainsExclusion(plans, "DTH", "DTHAC", "Hang Gliding activities", true);
            AssertCoverOnPlanContainsExclusion(plans, "DTH", "DTHIC", "Hang Gliding activities (Illness Cover)", true);

            AssertCoverOnPlanContainsExclusion(plans, "TPS", "TPSASC", "Hang Gliding activities", true);
            AssertCoverOnPlanContainsExclusion(plans, "TPS", "TPSAC", "Hang Gliding activities", true);
            AssertCoverOnPlanContainsExclusion(plans, "TPS", "TPSIC", "Hang Gliding activities", true);

            AssertCoverOnPlanContainsExclusion(plans, "TRS", "TRSCC", "Hang Gliding activities (Cancer Cover)", true);
            AssertCoverOnPlanContainsExclusion(plans, "TRS", "TRSSIC", "Hang Gliding activities (Critical Illness Cover)", true);
            AssertCoverOnPlanContainsExclusion(plans, "TRS", "TRSSIN", "Hang Gliding activities", true);

            AssertCoverOnPlanContainsExclusion(plans, "IP", "IPSAC", "Hang Gliding activities", true);
            AssertCoverOnPlanContainsExclusion(plans, "IP", "IPSSC", "Hang Gliding activities", true);
            AssertCoverOnPlanContainsExclusion(plans, "IP", "IPSIC", "Hang Gliding activities", true);
        }


        [Test]
        public async Task GetReview_ForRiskWithWeightLoading_ReturnsCorrectPremium_Async()
        {
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            await customerUnderwriting.AnswerToGetWeightLoading();
            FinaliseCustomerUnderwriting(risk.Id);

            var planDetails =
                Container.Resolve<IPlanDetailsService>()
                    .GetPlanDetailsForRisk(QuoteSessionContext.QuoteSession.QuoteReference, risk.Id);

            var response = await Client.GetReviewForRiskAsync(risk.Id);

            var planDetailsLifeIllnessPremium =
                planDetails.Plans.Single(p => p.PlanCode == "DTH").Covers.Single(c => c.Code == "DTHIC").Premium;

            var reviewLifeIllnessPremium = response.Cover.Plans.Single(p => p.PlanCode == "DTH").Covers.Single(c => c.Code == "DTHIC").Premium;

            Assert.That(planDetailsLifeIllnessPremium, Is.EqualTo(reviewLifeIllnessPremium));
        }

        [Test, Description("Scenario where customer is loaded but returns a zero loading dollar amount cause we can't calculate it")]
        public async Task GetReview_ForRiskWithLoadingZeroDollarAmount_ReturnsLoadings_Async()
        {
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            await customerUnderwriting.AnswerToGetKidneyStonesLoadingAsync();
            await customerUnderwriting.AnswerToGetCholesterolLoading();
            await customerUnderwriting.AnswerToGetHyperthyroidismLoading();
            FinaliseCustomerUnderwriting(risk.Id);

            var response = await Client.GetReviewForRiskAsync(risk.Id);

            var hyperthyroidismLoading = response.SharedLoadings.Single(l => l.Name == "Hyperthyroidism");
            Assert.That(hyperthyroidismLoading.ZeroLoadingCalcuation, Is.True);

            var cholesteralLoading = response.SharedLoadings.Single(l => l.Name == "High Cholesterol");
            Assert.That(cholesteralLoading.ZeroLoadingCalcuation, Is.True);

            var kidneyStoneLoading = response.SharedLoadings.Single(l => l.Name == "Kidney Stone");
            Assert.That(kidneyStoneLoading.ZeroLoadingCalcuation, Is.False);
        }

        [Test]
        public async Task SubmitReview_WhenAllCoversIneligible_ReturnsSuccessAndRedirectUrl_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();
            var risk = risks.First();
            var customerUnderwriting = new CustomerRiskUnderwritingHelper(risk.Id, _qualificationClient);

            //Make all plans ineligible
            await customerUnderwriting.MakeUnderwritingGlobalDecline();
            FinaliseCustomerUnderwriting(risk.Id);

            var response = await Client.ValidateReviewForRiskAsync(risk.Id);
            Assert.That(response.RedirectTo, Is.EqualTo("/Submission"));

            var plansResponse = await GetPlansForRisk(risk.Id);
            var plans = plansResponse.ToList();

            Assert.That(RiskMarketingStatusIsEqualTo(MarketingStatus.Decline, risks.First().Id), Is.True);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[0].PlanId), Is.True);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[1].PlanId), Is.True);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[2].PlanId), Is.True);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[3].PlanId), Is.True);
        }

        public void AssertCoverOnPlanContainsExclusion(IEnumerable<PlanSelectionResponse> plans, string planCode,
            string coverCode, string exclusionName, bool containsExclusion)
        {
            var plan = plans.Single(p => p.PlanCode == planCode);
            var cover = plan.Covers.Single(c => c.Code == coverCode);
            Assert.That(cover.ExclusionNames.Contains(exclusionName), Is.EqualTo(containsExclusion), $"Cover '{cover.Code}' does not contain exclusion '{exclusionName}'");
        }
    }
}

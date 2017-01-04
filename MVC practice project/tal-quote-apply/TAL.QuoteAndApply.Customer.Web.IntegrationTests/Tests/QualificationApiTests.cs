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
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests
{
    [TestFixture]
    public class QualificationApiTests : BaseTestClass<QualificationApiClient>
    {
        public QualificationApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }


        [Test]
        public async Task GetUnderwritingRisks_WhenCalled_ReturnsRiskIds_Async()
        {
            //Arrange
            await BasicInfoDefaultLoginAsync();

            //Act
            var response = await Client.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>();

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Any(), Is.True);
        }

        [Test]
        public async Task GetUnderwritingForRisk_WhenCalled_ReturnsUnderwritingModelApiModelWithSomeQuestions_Async()
        {
            //Arrange
            await BasicInfoDefaultLoginAsync();
            var getRisksResponse = await Client.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>();
            var risk = getRisksResponse.First();

            //Act

            var response = await Client.GetUnderwritingForRiskAsync<CustomerUnderwritingResponse>(risk.Id);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Questions.Any(), Is.True);
        }


        [Test]
        public async Task GetUnderwriting_WhenCalled_ReturnsQuestionsIncludingKnownQuestions_Async()
        {
            //Arrange
            await BasicInfoDefaultLoginAsync();
            var getRisksResponse = await Client.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>();
            var risk = getRisksResponse.First();

            //Act
            var response = await Client.GetUnderwritingForRiskAsync<CustomerUnderwritingResponse>(risk.Id);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Questions.Any(), Is.True);
            Assert.That(response.Questions.SingleOrDefault(q => q.Id == QuestionIds.SmokerQuestion), Is.Not.Null);
            Assert.That(response.Questions.SingleOrDefault(q => q.Id == QuestionIds.DateOfBirthQuestion), Is.Not.Null);
            Assert.That(response.Questions.SingleOrDefault(q => q.Id == QuestionIds.GenderQuestion), Is.Not.Null);
            Assert.That(response.Questions.SingleOrDefault(q => q.Id == QuestionIds.AnnualIncomeQuestion), Is.Not.Null);
        }
        
        [Test]
        public async Task AnswerQuestion_AnswerDisclosureQuestion_ReturnsSuccessfulResult_Async()
        {
            //Arrange
            await BasicInfoDefaultLoginAsync();
            var getRisksResponse = await Client.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>();
            var risk = getRisksResponse.First();
            var riskUnderwritingHelper = new CustomerRiskUnderwritingHelper(risk.Id, Client);

            //Act
            var answerQuestionResponse = await riskUnderwritingHelper.DisclosureQuestion().AnswerByTextAsync("Yes");

            //Assert
            Assert.That(answerQuestionResponse, Is.Not.Null);
            Assert.That(answerQuestionResponse.AddedQuestions, Is.Not.Null);
            Assert.That(answerQuestionResponse.ChangedQuestions, Is.Not.Null);
            Assert.That(answerQuestionResponse.RemovedQuestionIds, Is.Not.Null);
        }


        [Test]
        public async Task AnswerQuestion_ReAnswerQuestion_ReturnsCorrectChangedQuestionResponses_Async()
        {
            //Arrange
            await BasicInfoDefaultLoginAsync();
            var getRisksResponse = await Client.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>();
            var risk = getRisksResponse.First();

            var riskUnderwritingHelper = new CustomerRiskUnderwritingHelper(risk.Id, Client);

            //Act
            await riskUnderwritingHelper.ResidencyQuestion().AnswerByTextAsync("Yes");
            var answerQuestionResponse = await riskUnderwritingHelper.ResidencyQuestion().AnswerByTextAsync("No");

            //Assert
            Assert.That(answerQuestionResponse.ChangedQuestions, Is.Not.Null);
            Assert.That(answerQuestionResponse.ChangedQuestions.Count(), Is.EqualTo(1));

            var changedQuestion = answerQuestionResponse.ChangedQuestions.First();
            Assert.That(changedQuestion.ChangedAnswers, Is.Not.Null);
            Assert.That(changedQuestion.ChangedAnswers.Count(), Is.EqualTo(2));

            Assert.That(changedQuestion.ChangedAnswers.First().ChangedAttributes["isSelected"], Is.EqualTo(false));
            Assert.That(changedQuestion.ChangedAnswers.First().ChangedAttributes["selectedId"], Is.Null);

            Assert.That(changedQuestion.ChangedAnswers.Last().ChangedAttributes["isSelected"], Is.EqualTo(true));
            Assert.That(changedQuestion.ChangedAnswers.Last().ChangedAttributes["selectedId"], Is.Not.Null);

        }
        

        [Test]
        public async Task AnswerQuestion_WhenAnsweringResidencyQuestion_ResidencyIsNotSyncedToDatabase_Async()
        {
            //Arrange
            await BasicInfoDefaultLoginAsync();

            var getRisksResponse = await Client.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>();
            var risk = getRisksResponse.First();

            var riskUnderwritingHelper = new CustomerRiskUnderwritingHelper(risk.Id, Client);

            //Act
            await riskUnderwritingHelper.QuestionById(QuestionIds.ResidencyQuestion).AnswerByTextAsync("Yes");

            //Assert
            var riskFromDb = RiskHelper.GetRisk(risk.Id, true);
            Assert.That(riskFromDb.Residency, Is.EqualTo(ResidencyStatus.Unknown));
        }

        [Test]
        public async Task AnswerQuestion_WhenDeclineUnderwriting_AndFinaliseUnderwritingIsCalled_PlanEligibilityUpdated_Async()
        {
            //Arrange
            await BasicInfoDefaultLoginAsync();

            var getRisksResponse = await Client.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>();
            var risk = getRisksResponse.First();

            var riskUnderwritingHelper = new CustomerRiskUnderwritingHelper(risk.Id, Client);

            //First assert that all plans are available
            var originalPlanDetails = GetPlanDetailsForRisk(QuoteSessionContext.QuoteSession.QuoteReference, risk.Id);
            Assert.That(originalPlanDetails.TotalPremium, Is.GreaterThan(0));
            Assert.That(originalPlanDetails.Plans.All(p => p.Availability.IsAvailable));

            await riskUnderwritingHelper.MakeUnderwritingGlobalDecline();

            //Assert that all plans are still available (since we haven't finalised underwriting)
            var afterReansweringQuestionPlanDetails = GetPlanDetailsForRisk(QuoteSessionContext.QuoteSession.QuoteReference, risk.Id);
            Assert.That(afterReansweringQuestionPlanDetails.Plans.All(p => p.Availability.IsAvailable));

            //Act            
            await Client.ValidateRisk(risk.Id);

            //Assert that now plans are ineligible
            var afterValidatePlanDetails = GetPlanDetailsForRisk(QuoteSessionContext.QuoteSession.QuoteReference, risk.Id);
            Assert.That(afterValidatePlanDetails.Plans.All(p => p.Availability.IsAvailable), Is.False);
            Assert.That(afterValidatePlanDetails.Plans.All(p => p.IsSelected), Is.False);
            Assert.That(afterValidatePlanDetails.TotalPremium, Is.EqualTo(0));
        }

        [Test]
        public async Task
            FinaliseUnderwriting_WhenPremiumWasAffectedDuringUnderwriting_NewPremiumUpdatedInDatabase_Async()
        {
            //Arrange
            await BasicInfoDefaultLoginAsync();

            var getRisksResponse = await Client.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>();
            var risk = getRisksResponse.First();

            var originalPremium = GetPlanDetailsForRisk(QuoteSessionContext.QuoteSession.QuoteReference, risk.Id).TotalPremium;

            var riskUnderwritingHelper = new CustomerRiskUnderwritingHelper(risk.Id, Client);
            await riskUnderwritingHelper.SmokerQuestion().AnswerYesAsync(); //Answer smoker to get higher premium

            //Act
            await Client.ValidateRisk(risk.Id);

            //Assert
            var updatedPremium = GetPlanDetailsForRisk(QuoteSessionContext.QuoteSession.QuoteReference, risk.Id).TotalPremium;

            Assert.That(updatedPremium, Is.GreaterThan(originalPremium));

            //Had a scenario where total premium returned from PolicyOverviewProvider was different than premium from PlanDetailsService
            //So testing they are the same here
            var premiumFromPolicyOverviewProvider =
                Container.Resolve<IPolicyOverviewProvider>().GetFor(QuoteSessionContext.QuoteSession.QuoteReference).Premium;

            Assert.That(updatedPremium, Is.EqualTo(premiumFromPolicyOverviewProvider)); 
        }

        private GetPlanResponse GetPlanDetailsForRisk(string quoteReference, int riskId)
        {
            var planDetailsService = Container.Resolve<IPlanDetailsService>();
            return planDetailsService.GetPlanDetailsForRisk(quoteReference, riskId);
        }

    }
}

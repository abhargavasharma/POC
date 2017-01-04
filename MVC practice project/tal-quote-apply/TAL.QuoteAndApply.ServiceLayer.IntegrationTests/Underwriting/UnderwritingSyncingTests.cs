using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Underwriting
{
    [TestFixture]
    public class UnderwritingSyncingTests : BaseServiceLayerTest
    {
        [SetUp]
        public void Setup()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();
        }

        [Test]
        public void GetInterview_StatusIsDeclinedInInterview_CoversUnderwritingStatusUpdated()
        {
            //arrange
            var createQuoteResult = CreateQuote();
            var policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertCoversAreInStatus(policy.Risks, UnderwritingStatus.Incomplete);

            var risk = policy.Risks.First().Risk;

            //this answers a question in TALUS without running our syncing code
            //this proves that "GetInterview" syncs
            UpdateUnderwritingQuestionDirectlyInTalus(risk, QuestionIds.ResidencyQuestion, "3", "No");

            //act
            GetInterviewCausingSync(risk);

            //assert
            policy = GetPolicy(createQuoteResult.QuoteReference);
            AssertCoversAreInStatus(policy.Risks, UnderwritingStatus.Decline);
        }

        [Test]
        public void GetInterview_LoadingsInInterview_CoversLoadingsUpdated()
        {
            //arrange
            var createQuoteResult = CreateQuote();
            var policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertNoCoverLoadings(policy.Risks);

            var risk = policy.Risks.First().Risk;

            //this answers a question in TALUS without running our syncing code
            //this proves that "GetInterview" syncs
            UpdateUnderwritingQuestionDirectlyInTalus(risk, "Sports and Travel?uj", "2", "Yes");
            UpdateUnderwritingQuestionDirectlyInTalus(risk, "Sports and Travel?uj?2?18q", "1w0", "Canyoning");
            UpdateUnderwritingQuestionDirectlyInTalus(risk, "Sports and Travel?uj?2?18q?1w0?14e", "2py", "Include canyoning activities at an additional cost");

            //act
            GetInterviewCausingSync(risk);

            //assert
            policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertThatCoverHasLoading(policy.Risks, "DTH", "DTHAC", LoadingType.PerMille, 2.0m);
            AssertThatCoverDoesntHaveLoading(policy.Risks, "DTH", "DTHIC", LoadingType.PerMille, 2.0m);
            AssertThatCoverDoesntHaveLoading(policy.Risks, "DTH", "DTHASC", LoadingType.PerMille, 2.0m);
        }

        [Test]
        public void GetInterview_ExclusionsInInterview_CoversExclusionsUpdated()
        {
            //arrange
            var createQuoteResult = CreateQuote();
            var policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertNoCoverLoadings(policy.Risks);

            var risk = policy.Risks.First().Risk;

            //this answers a question in TALUS without running our syncing code
            //this proves that "GetInterview" syncs
            UpdateUnderwritingQuestionDirectlyInTalus(risk, "Sports and Travel?uj", "2", "Yes");
            UpdateUnderwritingQuestionDirectlyInTalus(risk, "Sports and Travel?uj?2?18q", "1w0", "Canyoning");
            UpdateUnderwritingQuestionDirectlyInTalus(risk, "Sports and Travel?uj?2?18q?1w0?14e", "2px", "Exclude canyoning activities");

            //act
            GetInterviewCausingSync(risk);

            //assert
            policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertThatCoverHasExclusion(policy.Risks, "DTH", "DTHAC", "Canyoning");
            AssertThatCoverHasExclusion(policy.Risks, "DTH", "DTHIC", "Canyoning (Illness Cover)");
            AssertThatCoverHasExclusion(policy.Risks, "DTH", "DTHASC", "Canyoning (Adventure Sports Cover)");

        }

        [Test]
        public void AnswerQuestionWithSync_ExclusionsInInterview_CoversExclusionsUpdated()
        {
            //arrange
            var createQuoteResult = CreateQuote();
            var policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertNoCoverLoadings(policy.Risks);

            var risk = policy.Risks.First().Risk;

            //act
            UpdateUnderwritingQuestionAndSync(risk, "Sports and Travel?uj", "2", "Yes");
            UpdateUnderwritingQuestionAndSync(risk, "Sports and Travel?uj?2?18q", "1w0", "Canyoning");
            UpdateUnderwritingQuestionAndSync(risk, "Sports and Travel?uj?2?18q?1w0?14e", "2px", "Exclude canyoning activities");

            //assert
            policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertThatCoverHasExclusion(policy.Risks, "DTH", "DTHAC", "Canyoning");
            AssertThatCoverHasExclusion(policy.Risks, "DTH", "DTHIC", "Canyoning (Illness Cover)");
            AssertThatCoverHasExclusion(policy.Risks, "DTH", "DTHASC", "Canyoning (Adventure Sports Cover)");
        }

        [Test]
        public void AnswerQuestionNoSync_ExclusionsInInterview_CoversExclusionsNotUpdated()
        {
            //arrange
            var createQuoteResult = CreateQuote();
            var policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertNoCoverLoadings(policy.Risks);

            var risk = policy.Risks.First().Risk;

            //act
            UpdateUnderwritingQuestionWithoutSync(risk, "Sports and Travel?uj", "2", "Yes");
            UpdateUnderwritingQuestionWithoutSync(risk, "Sports and Travel?uj?2?18q", "1w0", "Canyoning");
            UpdateUnderwritingQuestionWithoutSync(risk, "Sports and Travel?uj?2?18q?1w0?14e", "2px", "Exclude canyoning activities");

            //assert
            policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertThatCoverDoesntHaveExclusion(policy.Risks, "DTH", "DTHAC", "Canyoning");
            AssertThatCoverDoesntHaveExclusion(policy.Risks, "DTH", "DTHIC", "Canyoning (Illness Cover)");
            AssertThatCoverDoesntHaveExclusion(policy.Risks, "DTH", "DTHASC", "Canyoning (Adventure Sports Cover)");

        }

        [Test]
        public void AnswerQuestionWithSync_LoadingsInInterview_CoversLoadingsUpdated()
        {
            //arrange
            var createQuoteResult = CreateQuote();
            var policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertNoCoverLoadings(policy.Risks);

            var risk = policy.Risks.First().Risk;

            //act
            UpdateUnderwritingQuestionAndSync(risk, "Sports and Travel?uj", "2", "Yes");
            UpdateUnderwritingQuestionAndSync(risk, "Sports and Travel?uj?2?18q", "1w0", "Canyoning");
            UpdateUnderwritingQuestionAndSync(risk, "Sports and Travel?uj?2?18q?1w0?14e", "2py", "Include canyoning activities at an additional cost");

            //assert
            policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertThatCoverHasLoading(policy.Risks, "DTH", "DTHAC", LoadingType.PerMille, 2.0m);
            AssertThatCoverDoesntHaveLoading(policy.Risks, "DTH", "DTHIC", LoadingType.PerMille, 2.0m);
            AssertThatCoverDoesntHaveLoading(policy.Risks, "DTH", "DTHASC", LoadingType.PerMille, 2.0m);
        }


        [Test]
        public void AnswerQuestionNoSync_LoadingsInInterview_CoversLoadingsNotUpdated()
        {
            //arrange
            var createQuoteResult = CreateQuote();
            var policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertNoCoverLoadings(policy.Risks);

            var risk = policy.Risks.First().Risk;

            //act
            UpdateUnderwritingQuestionWithoutSync(risk, "Sports and Travel?uj", "2", "Yes");
            UpdateUnderwritingQuestionWithoutSync(risk, "Sports and Travel?uj?2?18q", "1w0", "Canyoning");
            UpdateUnderwritingQuestionWithoutSync(risk, "Sports and Travel?uj?2?18q?1w0?14e", "2py", "Include canyoning activities at an additional cost");

            //assert
            policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertNoCoverLoadings(policy.Risks);
        }

        [Test]
        public void AnswerQuestion__LoadingsInInterview_CoversLoadingsUpdated()
        {
            //arrange
            var createQuoteResult = CreateQuote();
            var policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertNoCoverLoadings(policy.Risks);

            var risk = policy.Risks.First().Risk;

            //act
            UpdateUnderwritingQuestionAndSync(risk, "Sports and Travel?uj", "2", "Yes");
            UpdateUnderwritingQuestionAndSync(risk, "Sports and Travel?uj?2?18q", "1w0", "Canyoning");
            UpdateUnderwritingQuestionAndSync(risk, "Sports and Travel?uj?2?18q?1w0?14e", "2py", "Include canyoning activities at an additional cost");

            //assert
            policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertThatCoverHasLoading(policy.Risks, "DTH", "DTHAC", LoadingType.PerMille, 2.0m);
            AssertThatCoverDoesntHaveLoading(policy.Risks, "DTH", "DTHIC", LoadingType.PerMille, 2.0m);
            AssertThatCoverDoesntHaveLoading(policy.Risks, "DTH", "DTHASC", LoadingType.PerMille, 2.0m);
        }

        [Test]
        public void AnswerQuestion_KnownQuestionsAnswered_RatingFactorsUpdated()
        {
            //arrange
            var createQuoteResult = CreateQuote();
            var policy = GetPolicy(createQuoteResult.QuoteReference);

            AssertNoCoverLoadings(policy.Risks);

            var risk = policy.Risks.First().Risk;

            //act
            UpdateUnderwritingQuestionAndSync(risk, QuestionIds.DateOfBirthQuestion, "*", "01/01/1968");
            UpdateUnderwritingQuestionAndSync(risk, QuestionIds.SmokerQuestion, "2", "Yes");
            UpdateUnderwritingQuestionAndSync(risk, QuestionIds.AnnualIncomeQuestion, "*", "123456");
            UpdateUnderwritingQuestionAndSync(risk, QuestionIds.GenderQuestion, "h2", "Female");
            UpdateUnderwritingQuestionAndSync(risk, QuestionIds.ResidencyQuestion, "2", "Yes");
            //TODO: answer occupation question once it's hooked up to DB

            //assert
            policy = GetPolicy(createQuoteResult.QuoteReference);
            var updatedRisk = policy.Risks.Single(r => r.Risk.Id == risk.Id).Risk;
            Assert.That(updatedRisk.DateOfBirth.ToFriendlyString(), Is.EqualTo("01/01/1968"));
            Assert.That(updatedRisk.SmokerStatus, Is.EqualTo(SmokerStatus.Yes));
            Assert.That(updatedRisk.AnnualIncome, Is.EqualTo(123456));
            Assert.That(updatedRisk.Gender, Is.EqualTo(Gender.Female));
            Assert.That(updatedRisk.Residency, Is.EqualTo(ResidencyStatus.Australian));
            //TODO: assert occupation synced correctly
        }


        private void AssertThatCoverHasLoading(IEnumerable<RiskWithPlans> risks, string planCode, string coverCode, LoadingType expectedLoadingType, decimal expectedLoadingAmount)
        {
            var coverLoadingSvc = GetServiceInstance<ICoverLoadingService>();

            foreach (var r in risks)
            {
                foreach (var p in r.Plans.Where(p=> p.Plan.Code == planCode))
                {
                    var cover = p.Covers.FirstOrDefault(c => c.Code == coverCode);

                    var coverLoadings = coverLoadingSvc.GetCoverLoadingsForCover(cover);

                    var loadingForType = coverLoadings.FirstOrDefault(l => l.LoadingType == expectedLoadingType);
                    Assert.That(loadingForType, Is.Not.Null);
                    Assert.That(loadingForType.Loading, Is.EqualTo(expectedLoadingAmount));
                }
            }
        }

        private void AssertThatCoverDoesntHaveLoading(IEnumerable<RiskWithPlans> risks, string planCode, string coverCode, LoadingType expectedLoadingType, decimal expectedLoadingAmount)
        {
            var coverLoadingSvc = GetServiceInstance<ICoverLoadingService>();

            foreach (var r in risks)
            {
                foreach (var p in r.Plans.Where(p => p.Plan.Code == planCode))
                {
                    var cover = p.Covers.FirstOrDefault(c => c.Code == coverCode);

                    var coverLoadings = coverLoadingSvc.GetCoverLoadingsForCover(cover);

                    var loadingForType = coverLoadings.FirstOrDefault(l => l.LoadingType == expectedLoadingType);
                    Assert.That(loadingForType, Is.Null);
                }
            }
        }

        private void AssertThatCoverHasExclusion(IEnumerable<RiskWithPlans> risks, string planCode, string coverCode, string exclusionName)
        {
            var coverExclusionsSvc = GetServiceInstance<ICoverExclusionsService>();

            foreach (var r in risks)
            {
                foreach (var p in r.Plans.Where(p => p.Plan.Code == planCode))
                {
                    var cover = p.Covers.FirstOrDefault(c => c.Code == coverCode);

                    var coverLoadings = coverExclusionsSvc.GetExclusionsForCover(cover);

                    var exclusion = coverLoadings.FirstOrDefault(e => e.Name == exclusionName);
                    Assert.That(exclusion, Is.Not.Null);
                }
            }
        }

        private void AssertThatCoverDoesntHaveExclusion(IEnumerable<RiskWithPlans> risks, string planCode, string coverCode, string exclusionName)
        {
            var coverExclusionsSvc = GetServiceInstance<ICoverExclusionsService>();

            foreach (var r in risks)
            {
                foreach (var p in r.Plans.Where(p => p.Plan.Code == planCode))
                {
                    var cover = p.Covers.FirstOrDefault(c => c.Code == coverCode);

                    var coverLoadings = coverExclusionsSvc.GetExclusionsForCover(cover);

                    var exclusion = coverLoadings.FirstOrDefault(e => e.Name == exclusionName);
                    Assert.That(exclusion, Is.Null);
                }
            }
        }

        private void AssertNoCoverLoadings(IEnumerable<RiskWithPlans> risks)
        {
            var coverLoadingSvc = GetServiceInstance<ICoverLoadingService>();

            foreach (var r in risks)
            {
                foreach (var p in r.Plans)
                {
                    foreach (var c in p.Covers)
                    {
                        var coverLoadings = coverLoadingSvc.GetCoverLoadingsForCover(c);

                        CollectionAssert.IsEmpty(coverLoadings);
                    }
                }
            }
        }

        private void UpdateUnderwritingQuestionAndSync(IRisk risk, string questionId, string responseId, string responseText)
        {
            var underwritingChangeSubject = GetServiceInstance<IUnderwritingBenefitsResponseChangeSubject>();
            var svc = GetServiceInstance<IRiskUnderwritingQuestionService>();

            svc.AnswerQuestionAndSync(risk.Id, questionId,
                new[] { new UnderwritingAnswer(responseId, responseText, "", true) });
        }

        private void UpdateUnderwritingQuestionWithoutSync(IRisk risk, string questionId, string responseId, string responseText)
        {
            var underwritingChangeSubject = GetServiceInstance<IUnderwritingBenefitsResponseChangeSubject>();
            var svc = GetServiceInstance<IRiskUnderwritingQuestionService>();

            svc.AnswerQuestionWithoutSyncing(risk.Id, questionId,
                new[] { new UnderwritingAnswer(responseId, responseText, "", true) });
        }

        private void GetInterviewCausingSync(IRisk risk)
        {
            var riskSvc = GetServiceInstance<IRiskService>();
            risk = riskSvc.GetRisk(risk.Id);

            var underwritingChangeSubject = GetServiceInstance<IUnderwritingBenefitsResponseChangeSubject>();

            var svc = GetServiceInstance<IGetUnderwritingInterview>();
            svc.GetInterview(risk.InterviewId, risk.InterviewConcurrencyToken, underwritingChangeSubject);
        }

        private void UpdateUnderwritingQuestionDirectlyInTalus(IRisk risk, string questionId, string responseId, string responseText)
        {
            var riskSvc = GetServiceInstance<IRiskService>();
            risk = riskSvc.GetRisk(risk.Id);

            var concurrencyToken = UnderwritingHelper.UpdateUnderwritingQuestionDirectlyInTalus(risk.InterviewId,  risk.InterviewConcurrencyToken, questionId, responseId, responseText);
            
            riskSvc.SetLatestConcurrencyToken(risk, concurrencyToken);
        }

        private void AssertCoversAreInStatus(IEnumerable<RiskWithPlans> risks, UnderwritingStatus uwStatus)
        {
            foreach (var r in risks)
            {
                foreach (var p in r.Plans)
                {
                    foreach (var c in p.Covers)
                    {
                        Assert.That(c.UnderwritingStatus, Is.EqualTo(uwStatus));
                    }
                }
            }
        }

        private PolicyWithRisks GetPolicy(string quoteRef)
        {
            var svc = GetServiceInstance<IPolicyWithRisksService>();

            return svc.GetFrom(quoteRef);
        }

        private CreateQuoteResult CreateQuote()
        {
            var createQuoteParam = new CreateQuoteParam(
                new RatingFactorsParam('M', DateTime.Today.AddYears(-(19)), true, new SmokerStatusHelper(false), "229", "27l", 100000),
                new PersonalInformationParam("Mr", "BLAH", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), false, Models.PolicySource.CustomerPortalBuildMyOwn, "TAL");

            var createQuoteService = GetServiceInstance<ICreateQuoteService>();

            return createQuoteService.CreateQuote(createQuoteParam);
        }

    }
}

using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Exceptions;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.Underwriting;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Underwriting
{
    [TestFixture]
    public class RiskUnderwritingAnswerSyncServiceTests
    {
        private Mock<IKnownQuestionIdProvider> _mockKnownQuestionIdProvider;
        private Mock<IRiskService> _mockRiskService;
        private Mock<IGetUnderwritingInterview> _mockGetUnderWritingInterview;
        private Mock<IDateOfBirthChangeSubject> _mockDateOfBirthChangeSubject;
        private Mock<IGenderChangeSubject> _mockGenderChangeSubject;
        private Mock<IOccupationChangeSubject> _mockOccupationChangeSubject;
        private Mock<IResidencyChangeSubject> _mockResidencyChangeSubject;
        private Mock<ISmokerStatusChangeSubject> _mockSmokerStatusChangeSubject;
        private Mock<IAnnualIncomeChangeSubject> _mockAnnualIncomeChangeSubject;
        private Mock<IUnderwritingBenefitsResponseChangeSubject> _mockUnderwritingBenefitsResponseChangeSubject;
        private Mock<IUnderwritingTagMetaDataService> _mockTagMetaDataService;
        private Mock<IPlanAutoUpdateService> _mockPlanAutoUpdateService;

        [TestFixtureSetUp]
        protected void Setup()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);
            _mockKnownQuestionIdProvider = mockRepository.Create<IKnownQuestionIdProvider>();
            _mockRiskService = mockRepository.Create<IRiskService>();
            _mockGetUnderWritingInterview = mockRepository.Create<IGetUnderwritingInterview>();
            _mockDateOfBirthChangeSubject = mockRepository.Create<IDateOfBirthChangeSubject>();
            _mockGenderChangeSubject = mockRepository.Create<IGenderChangeSubject>();
            _mockOccupationChangeSubject = mockRepository.Create<IOccupationChangeSubject>();
            _mockResidencyChangeSubject = mockRepository.Create<IResidencyChangeSubject>();
            _mockSmokerStatusChangeSubject = mockRepository.Create<ISmokerStatusChangeSubject>();
            _mockAnnualIncomeChangeSubject = mockRepository.Create<IAnnualIncomeChangeSubject>();
            _mockUnderwritingBenefitsResponseChangeSubject = mockRepository.Create<IUnderwritingBenefitsResponseChangeSubject>();
            _mockTagMetaDataService = mockRepository.Create<IUnderwritingTagMetaDataService>();
            _mockPlanAutoUpdateService = mockRepository.Create<IPlanAutoUpdateService>();
        }

        [Test, ExpectedException(typeof(ServiceLayerException))]
        public void SyncRiskWithUnderwritingAnswer_RiskNotReturned_ExceptionThrown()
        {
            const int riskId = 1;
            const string concurrencyToken = "ABC123";
            string questionId = QuestionIds.EmploymentQuestion;
            var underwritingQuestionAnswer = new UnderwritingQuestionAnswer(questionId, new [] { new UnderwritingAnswer("1", "One") });

            _mockKnownQuestionIdProvider.Setup(call => call.GetAllKnownQuestionIds(It.IsAny<IRisk>())).Returns(GetMockKnownQuestionIds());
            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(() => null);

            var svc = GetService();
            svc.SyncRiskWithUnderwritingAnswer(riskId, concurrencyToken, underwritingQuestionAnswer);
        }

        [Test]
        public void SyncRiskWithUnderwritingAnswer_NotKnownQuestion_InteviewLoaded()
        {
            const int riskId = 1;
            const string questionId = "abc?123";
            var underwritingQuestionAnswer = new UnderwritingQuestionAnswer(questionId, new UnderwritingAnswer[] { new UnderwritingAnswer("1", "One") });

            var answer = new UnderwritingAnswer("1", "One");

            var mockInterview = GetMockInterview(questionId, answer);

            var mockRisk = new RiskDto();
            mockRisk.Id = riskId;
            mockRisk.InterviewId = "1234";
            mockRisk.InterviewConcurrencyToken = "ABC123";

            _mockKnownQuestionIdProvider.Setup(call => call.GetAllKnownQuestionIds(It.IsAny<IRisk>())).Returns(new string[] { });
            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockGetUnderWritingInterview.Setup(call => call.GetInterview(mockRisk.InterviewId, mockRisk.InterviewConcurrencyToken, _mockUnderwritingBenefitsResponseChangeSubject.Object)).Returns(mockInterview);
            _mockRiskService.Setup(call => call.SetLatestConcurrencyToken(mockRisk, mockRisk.InterviewConcurrencyToken));
            _mockPlanAutoUpdateService.Setup(call => call.UpdatePlansToConformWithPlanEligiblityRules(mockRisk));

            var svc = GetService();
            svc.SyncRiskWithUnderwritingAnswer(riskId, mockRisk.InterviewConcurrencyToken, underwritingQuestionAnswer);
        }

        [Test]
        public void SyncRiskWithUnderwritingAnswer_KnownQuestionEmploymentQuestion_OccupationUpdatedOnRisk()
        {
            const int riskId = 1;
            string questionId = QuestionIds.EmploymentQuestion;
            const string occupationRating = "AAA";

            var answer = new UnderwritingAnswer("1", "One");

            var underwritingQuestionAnswer = new UnderwritingQuestionAnswer(questionId, new [] { answer });

            var mockRisk = new RiskDto();
            mockRisk.InterviewId = "1234";
            mockRisk.InterviewConcurrencyToken = "ABC123";

            var mockInterview = GetMockInterview(questionId, answer, new KeyValuePair<string, string>(VariableConstants.OccupationClass, occupationRating));

            _mockKnownQuestionIdProvider.Setup(call => call.GetAllKnownQuestionIds(It.IsAny<IRisk>())).Returns(GetMockKnownQuestionIds());
            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockGetUnderWritingInterview.Setup(call => call.GetInterview(mockRisk.InterviewId, mockRisk.InterviewConcurrencyToken, _mockUnderwritingBenefitsResponseChangeSubject.Object)).Returns(mockInterview);
            _mockOccupationChangeSubject.Setup(call => call.Notify(It.IsAny<ChangeEnvelope>()));
            _mockRiskService.Setup(call => call.SetLatestConcurrencyToken(mockRisk, mockRisk.InterviewConcurrencyToken));
            _mockPlanAutoUpdateService.Setup(call => call.UpdatePlansToConformWithPlanEligiblityRules(mockRisk));

            var svc = GetService();
            svc.SyncRiskWithUnderwritingAnswer(riskId, mockRisk.InterviewConcurrencyToken, underwritingQuestionAnswer);
        }

        [Test]
        public void SyncRiskWithUnderwritingAnswer_KnownQuestionDateOfBirthQuestion_DateOfBirthChangeNotified()
        {
            const int riskId = 1;
            string questionId = QuestionIds.DateOfBirthQuestion;
            var answer = new UnderwritingAnswer("1", "10/10/1980");

            var underwritingQuestionAnswer = new UnderwritingQuestionAnswer(questionId, new[] { answer });

            var mockRisk = new RiskDto();
            mockRisk.InterviewId = "1234";
            mockRisk.InterviewConcurrencyToken = "ABC123";

            var mockInterview = GetMockInterview(questionId, answer);

            _mockKnownQuestionIdProvider.Setup(call => call.GetAllKnownQuestionIds(It.IsAny<IRisk>())).Returns(GetMockKnownQuestionIds());
            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockGetUnderWritingInterview.Setup(call => call.GetInterview(mockRisk.InterviewId, mockRisk.InterviewConcurrencyToken, _mockUnderwritingBenefitsResponseChangeSubject.Object)).Returns(mockInterview);
            _mockDateOfBirthChangeSubject.Setup(call => call.Notify(It.IsAny<ChangeEnvelope>()));
            _mockRiskService.Setup(call => call.SetLatestConcurrencyToken(mockRisk, mockRisk.InterviewConcurrencyToken));
            _mockPlanAutoUpdateService.Setup(call => call.UpdatePlansToConformWithPlanEligiblityRules(mockRisk));

            var svc = GetService();
            svc.SyncRiskWithUnderwritingAnswer(riskId, mockRisk.InterviewConcurrencyToken, underwritingQuestionAnswer);
        }

        [Test]
        public void SyncRiskWithUnderwritingAnswer_KnownQuestionGenderQuestion_GenderUpdatedOnRisk()
        {
            const int riskId = 1;
            string questionId = QuestionIds.GenderQuestion;
            var answer = new UnderwritingAnswer("1", "Female");

            var underwritingQuestionAnswer = new UnderwritingQuestionAnswer(questionId, new[] { answer });

            var mockRisk = new RiskDto();
            mockRisk.InterviewId = "1234";
            mockRisk.InterviewConcurrencyToken = "ABC123";

            var mockInterview = GetMockInterview(questionId, answer);

            _mockKnownQuestionIdProvider.Setup(call => call.GetAllKnownQuestionIds(It.IsAny<IRisk>())).Returns(GetMockKnownQuestionIds());
            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockGetUnderWritingInterview.Setup(call => call.GetInterview(mockRisk.InterviewId, mockRisk.InterviewConcurrencyToken, _mockUnderwritingBenefitsResponseChangeSubject.Object)).Returns(mockInterview);
            _mockGenderChangeSubject.Setup(call => call.Notify(It.IsAny<ChangeEnvelope>()));
            _mockRiskService.Setup(call => call.SetLatestConcurrencyToken(mockRisk, mockRisk.InterviewConcurrencyToken));
            _mockPlanAutoUpdateService.Setup(call => call.UpdatePlansToConformWithPlanEligiblityRules(mockRisk));

            var svc = GetService();
            svc.SyncRiskWithUnderwritingAnswer(riskId, mockRisk.InterviewConcurrencyToken, underwritingQuestionAnswer);
        }

        [Test]
        public void SyncRiskWithUnderwritingAnswer_KnownQuestionResidencyQuestion_ResidencyUpdatedOnRisk()
        {
            const int riskId = 1;
            string questionId = QuestionIds.ResidencyQuestion;
            var answer = new UnderwritingAnswer("1", "Yes");

            var underwritingQuestionAnswer = new UnderwritingQuestionAnswer(questionId, new[] { answer });

            var mockRisk = new RiskDto();
            mockRisk.InterviewId = "1234";
            mockRisk.InterviewConcurrencyToken = "ABC123";

            var mockInterview = GetMockInterview(questionId, answer);

            _mockKnownQuestionIdProvider.Setup(call => call.GetAllKnownQuestionIds(It.IsAny<IRisk>())).Returns(GetMockKnownQuestionIds());
            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockGetUnderWritingInterview.Setup(call => call.GetInterview(mockRisk.InterviewId, mockRisk.InterviewConcurrencyToken, _mockUnderwritingBenefitsResponseChangeSubject.Object)).Returns(mockInterview);
            _mockResidencyChangeSubject.Setup(call => call.Notify(It.IsAny<ChangeEnvelope>()));
            _mockRiskService.Setup(call => call.SetLatestConcurrencyToken(mockRisk, mockRisk.InterviewConcurrencyToken));
            _mockPlanAutoUpdateService.Setup(call => call.UpdatePlansToConformWithPlanEligiblityRules(mockRisk));

            var svc = GetService();
            svc.SyncRiskWithUnderwritingAnswer(riskId, mockRisk.InterviewConcurrencyToken, underwritingQuestionAnswer);
        }

        [Test]
        public void SyncRiskWithUnderwritingAnswer_KnownQuestionSmokerQuestion_SmokerStatusUpdatedOnRisk()
        {
            const int riskId = 1;
            string questionId = QuestionIds.SmokerQuestion;
            var answer = new UnderwritingAnswer("1", "Yes");

            var underwritingQuestionAnswer = new UnderwritingQuestionAnswer(questionId, new[] { answer });

            var mockRisk = new RiskDto();
            mockRisk.InterviewId = "1234";
            mockRisk.InterviewConcurrencyToken = "ABC123";

            var mockInterview = GetMockInterview(questionId, answer);

            _mockKnownQuestionIdProvider.Setup(call => call.GetAllKnownQuestionIds(It.IsAny<IRisk>())).Returns(GetMockKnownQuestionIds());
            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockGetUnderWritingInterview.Setup(call => call.GetInterview(mockRisk.InterviewId, mockRisk.InterviewConcurrencyToken, _mockUnderwritingBenefitsResponseChangeSubject.Object)).Returns(mockInterview);
            _mockSmokerStatusChangeSubject.Setup(call => call.Notify(It.IsAny<ChangeEnvelope>()));
            _mockRiskService.Setup(call => call.SetLatestConcurrencyToken(mockRisk, mockRisk.InterviewConcurrencyToken));
            _mockPlanAutoUpdateService.Setup(call => call.UpdatePlansToConformWithPlanEligiblityRules(mockRisk));

            var svc = GetService();
            svc.SyncRiskWithUnderwritingAnswer(riskId, mockRisk.InterviewConcurrencyToken, underwritingQuestionAnswer);
        }

        [Test]
        public void SyncRiskWithUnderwritingAnswer_KnownQuestionAnnualIncomeQuestion_AnnualIncomeUpdatedOnRisk()
        {
            const int riskId = 1;
            string questionId = QuestionIds.AnnualIncomeQuestion;
            var answer = new UnderwritingAnswer("1", "1000");

            var underwritingQuestionAnswer = new UnderwritingQuestionAnswer(questionId, new[] { answer });

            var mockRisk = new RiskDto();
            mockRisk.InterviewId = "1234";
            mockRisk.InterviewConcurrencyToken = "ABC123";

            var mockInterview = GetMockInterview(questionId, answer);

            _mockKnownQuestionIdProvider.Setup(call => call.GetAllKnownQuestionIds(It.IsAny<IRisk>())).Returns(GetMockKnownQuestionIds());
            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockGetUnderWritingInterview.Setup(call => call.GetInterview(mockRisk.InterviewId, mockRisk.InterviewConcurrencyToken, _mockUnderwritingBenefitsResponseChangeSubject.Object)).Returns(mockInterview);
            _mockAnnualIncomeChangeSubject.Setup(call => call.Notify(It.IsAny<ChangeEnvelope>()));
            _mockRiskService.Setup(call => call.SetLatestConcurrencyToken(mockRisk, mockRisk.InterviewConcurrencyToken));
            _mockPlanAutoUpdateService.Setup(call => call.UpdatePlansToConformWithPlanEligiblityRules(mockRisk));

            var svc = GetService();
            svc.SyncRiskWithUnderwritingAnswer(riskId, mockRisk.InterviewConcurrencyToken, underwritingQuestionAnswer);
        }

        private ReadOnlyUnderwritingInterview GetMockInterview(string questionId, UnderwritingAnswer changedAnswer, KeyValuePair<string, string>? variable = null)
        {
            var answer = new Answer
            {
                Selected = true,
                SelectedId = changedAnswer.Id,
                SelectedText = changedAnswer.Text,
                Text = changedAnswer.Text
            };

            var question = new Question
            {
                Id = questionId,
                Answers = new List<Answer> {answer}
            };

            var benefitResponse = new InterviewBenefitResponse
            {
                TotalLoadings = new TotalLoadings(),
                AnsweredQuestions = new List<Question> { question },
                Variables = new Dictionary<string, string>()
            };

            if (variable.HasValue)
            {
                benefitResponse.Variables.Add(variable.Value.Key, variable.Value.Value);
            }

            var underwritingInterview = new UnderwritingInterview
            {
                Benefits = new List<InterviewBenefitResponse> {benefitResponse}
            };

            return new ReadOnlyUnderwritingInterview(underwritingInterview);
        }

        private string[] GetMockKnownQuestionIds()
        {
            return new[]
            {
                QuestionIds.DateOfBirthQuestion,
                QuestionIds.EmploymentQuestion,
                QuestionIds.GenderQuestion,
                QuestionIds.ResidencyQuestion,
                QuestionIds.SmokerQuestion,
                QuestionIds.AnnualIncomeQuestion
            };
        }

        private RiskUnderwritingAnswerSyncService GetService()
        {
            return new RiskUnderwritingAnswerSyncService(_mockKnownQuestionIdProvider.Object, 
                _mockRiskService.Object, 
                _mockGetUnderWritingInterview.Object, 
                _mockDateOfBirthChangeSubject.Object,
                _mockGenderChangeSubject.Object,
                _mockOccupationChangeSubject.Object,
                _mockResidencyChangeSubject.Object,
                _mockSmokerStatusChangeSubject.Object,
                _mockAnnualIncomeChangeSubject.Object,
                _mockUnderwritingBenefitsResponseChangeSubject.Object,
                _mockTagMetaDataService.Object,
                _mockPlanAutoUpdateService.Object);
        }

    }
}

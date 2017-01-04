using System;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.ServiceLayer.Exceptions;
using TAL.QuoteAndApply.ServiceLayer.Search.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.Tests.Shared.Mocks;
using TAL.QuoteAndApply.Underwriting.Configuration;
using TAL.QuoteAndApply.Underwriting.Models.Converters;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.Tests.Shared.Helpers
{
    public static class UnderwritingHelper
    {
        public const string OccupationCode_AccountExecutive = "229";
        public const string OccupationCode_ManualDuties = "22p";
        public const string OccupationCode_CommercialAirlinePilot = "2uu";
        public const string OccupationCode_WharfWorker = "2eb";

        public const string IndustryCode_AdvertisingAndMarketing = "27l";
        public const string IndustryCode_Aviation = "27n";
        public const string IndustryCode_Marine = "27y";

        public const string IndustryCode_InformationTechnology = "27v";
        public const string OccupationCode_ComputerAnalyst = "19c";

        private static readonly UnderwritingConfigurationProvider _underwritingConfigurationProvider;
        private static readonly UnderwritingTemplateService _underwritingTemplateService;
        private static readonly CreateUnderwritingInterview _createUnderwritingInterview;
        private static readonly GetUnderwritingInterview _getUnderwritingInterview;
        private static readonly UpdateUnderwritingInterview _updateUnderwritingInterview;
        private static readonly UnderwritingBenefitsResponseChangeSubject _underwritingBenefitsResponseChangeSubject;
        private static readonly UnderwritingQuestionAnswerProvider _underwritingQuestionAnswer;
        private static readonly IUnderwritingBenefitResponsesChangeParamConverter _underwritingBenefitResponsesChangeParamConverter = new UnderwritingBenefitResponsesChangeParamConverter();
        private static UnderwritingWebServiceUrlProvider _underwritingWebServiceUrlProvider;
        private static HttpClientService _httpClient;
        private static MockCurrentUserProvider _mockUserProvider;

        static UnderwritingHelper()
        {
            _mockUserProvider = new MockCurrentUserProvider();

            _httpClient = new HttpClientService(new HttpResponseMessageSerializer(), new MimeTypeProvider(), new HttpRequestMessageSerializer());

            _underwritingConfigurationProvider = new UnderwritingConfigurationProvider();

            _underwritingWebServiceUrlProvider =
                new UnderwritingWebServiceUrlProvider(_underwritingConfigurationProvider);

            _underwritingTemplateService =
                new UnderwritingTemplateService(
                    _underwritingWebServiceUrlProvider,
                    _httpClient);

            _createUnderwritingInterview = new CreateUnderwritingInterview(_httpClient, _underwritingWebServiceUrlProvider,
                new CreateInterviewRequestProvider(), _mockUserProvider);

            _getUnderwritingInterview = new GetUnderwritingInterview(_underwritingWebServiceUrlProvider, _httpClient, _underwritingBenefitResponsesChangeParamConverter, new MockCacheWrapper());
            _updateUnderwritingInterview = new UpdateUnderwritingInterview(_httpClient, _underwritingWebServiceUrlProvider, _mockUserProvider);
            _underwritingBenefitsResponseChangeSubject = new UnderwritingBenefitsResponseChangeSubject();

            _underwritingQuestionAnswer = new UnderwritingQuestionAnswerProvider(new CachingWrapper(new MockHttpProvider()));
        }

        public static ReadOnlyUnderwritingInterview CreateUnderwritingInterview()
        {
            var templateInfo = _underwritingTemplateService.GetTemplateInformation(_underwritingConfigurationProvider.TemplateName);

            return _createUnderwritingInterview.CreateNewInterview(_underwritingConfigurationProvider.TemplateName, 
                _underwritingConfigurationProvider.FullWorkflow, 
                templateInfo.Benefits);
        }

        public static ReadOnlyUnderwritingInterview GetInterview(string interviewId, string concurrencyToken)
        {
            return _getUnderwritingInterview.GetInterview(interviewId, concurrencyToken, _underwritingBenefitsResponseChangeSubject);
        }

        public static ReadOnlyAnswer GetAnswerToQuestionInInterview(string interviewId, string concurrencyToken, string questionId)
        {
            var interview = GetInterview(interviewId, concurrencyToken);
            var questions =
                interview.Benefits.SelectMany(
                    b => b.UnansweredQuestions.Concat(interview.Benefits.SelectMany(b1 => b1.AnsweredQuestions)))
                    .Distinct(DistinctQuestionComparer.Instance);

            var question = questions.FirstOrDefault(x => x.Id == questionId);
            if (question == null)
            {
                throw new ServiceLayerException("Underwriting question not found");
            }

            var answer = question.Answers.FirstOrDefault(x => x.Selected);
            if (answer == null)
            {
                throw new ServiceLayerException("Underwriting answer not found");
            }

            return answer;
        }

        public static void AnswerDateOfBirthQuestion(string interviewId, string concurrencyToken, DateTime dateOfBirth)
        {
            var interview = GetInterview(interviewId, concurrencyToken);
            var questions = _underwritingQuestionAnswer.GetQuestionByTag(QuestionTagConstants.DateOfBirthQuestionTag,
                interview.TemplateVersion, () => interview).Single();

            var q = new UnderwritingInitialiseFreetextQuestion(
                dateOfBirth.ToString("dd/MM/yyyy"), interview.InterviewIdentifier,
                questions, _updateUnderwritingInterview);

            q.AnswerQuestion(interview.ConcurrencyToken);
        }

        public static void AnswerAnnualIncomeQuestion(string interviewId, string concurrencyToken, long? annualIncome)
        {
            var annualIncomeStr = annualIncome.ToString();
            AnswerAnnualIncomeQuestion(interviewId, concurrencyToken, annualIncomeStr);
        }

        public static void AnswerAnnualIncomeQuestion(string interviewId, string concurrencyToken, decimal? annualIncome)
        {
            var annualIncomeStr = annualIncome.ToString();
            AnswerAnnualIncomeQuestion(interviewId, concurrencyToken, annualIncomeStr);
        }

        private static void AnswerAnnualIncomeQuestion(string interviewId, string concurrencyToken, string annualIncome)
        {
            var interview = GetInterview(interviewId, concurrencyToken);
            var questions = _underwritingQuestionAnswer.GetQuestionByTag(QuestionTagConstants.AnnualIncomeQuestionTag,
                interview.TemplateVersion, () => interview).Single();

            var q = new UnderwritingInitialiseFreetextQuestion(annualIncome, interview.InterviewIdentifier,
                questions, _updateUnderwritingInterview);

            q.AnswerQuestion(interview.ConcurrencyToken);
        }
        public static string UpdateUnderwritingQuestionDirectlyInTalus(string interviwId, string concurrencyToken, string questionId, string responseId, string responseText)
        {
            var request = new UpdateInterviewRequest(new AnswerQuestionRequest(questionId, new[] { new AnswerSubmission { ResponseId = responseId, Text = responseText } }, _mockUserProvider.GetForApplication().UserName));

            var urlString = _underwritingWebServiceUrlProvider.GetInterviewUrl(interviwId);

            var putRequest = new PutOrPostRequest(new Uri(urlString), request)
                .WithEtag(concurrencyToken);

            var updatedUnderwritingInterview = _httpClient.PutAsync<UpdatedUnderwritingInterview>(putRequest);

            return updatedUnderwritingInterview.Result.ConcurrencyToken;
        }

        public static string GetCurrentInterviewConcurrencyToken(string interviewId)
        {
            var urlString = _underwritingWebServiceUrlProvider.GetInterviewUrl(interviewId);

            var underwritingInterview = _httpClient.GetAsync<UnderwritingInterview>(new GetRequest(new Uri(urlString)));

            return underwritingInterview.Result.ConcurrencyToken;
        }

    }
}
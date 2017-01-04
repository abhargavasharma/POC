using System;
using System.Linq;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Service
{
    public interface IUpdateUnderwritingInterview
    {
        ReadOnlyUpdatedUnderwritingInterview AnswerQuestion(string underwritingId, string concurrencyToken,
            string questionPath, params AnswerSubmission[] answers);
        ReadOnlyUpdatedUnderwritingInterview AnswerQuestion(ReadOnlyUnderwritingInterview interview,
            string questionPath, params AnswerSubmission[] answers);
    }

    public class UpdateUnderwritingInterview : IUpdateUnderwritingInterview
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUnderwritingWebServiceUrlProvider _underwritingWebServiceUrlProvider;
        private readonly ICurrentUserProvider _currentUserProvider;

        public UpdateUnderwritingInterview(IHttpClientService httpClientService, 
            IUnderwritingWebServiceUrlProvider underwritingWebServiceUrlProvider,
            ICurrentUserProvider currentUserProvider)
        {
            _httpClientService = httpClientService;
            _underwritingWebServiceUrlProvider = underwritingWebServiceUrlProvider;
            _currentUserProvider = currentUserProvider;
        }

        public ReadOnlyUpdatedUnderwritingInterview AnswerQuestion(ReadOnlyUnderwritingInterview interview,
            string questionPath, params AnswerSubmission[] answers)
        {
            return AnswerQuestion(interview.InterviewIdentifier, interview.ConcurrencyToken, questionPath, answers);
        }

        public ReadOnlyUpdatedUnderwritingInterview AnswerQuestion(string underwritingId, 
            string concurrencyToken, string questionPath, params AnswerSubmission[] answers)
        {
            var request = new UpdateInterviewRequest(new AnswerQuestionRequest(questionPath, answers.ToList(), _currentUserProvider.GetForApplication().UserName));

            var urlString = _underwritingWebServiceUrlProvider.GetInterviewUrl(underwritingId);

            var putRequest = new PutOrPostRequest(new Uri(urlString), request)
                .WithEtag(concurrencyToken);

            var updatedUnderwritingInterview = _httpClientService.PutAsync<UpdatedUnderwritingInterview>(putRequest).Result;

            var readOnlyUpdatedUnderwritingInterview = new ReadOnlyUpdatedUnderwritingInterview(updatedUnderwritingInterview);

            return readOnlyUpdatedUnderwritingInterview;
        }
    }
}
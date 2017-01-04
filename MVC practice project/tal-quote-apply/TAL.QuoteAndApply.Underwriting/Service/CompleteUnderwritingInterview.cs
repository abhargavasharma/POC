using System;
using System.Net;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Service
{
    public interface ICompleteUnderwritingInterview
    {
        bool Complete(string underwritingId, string concurrencyToken);
    }

    public class CompleteUnderwritingInterview : ICompleteUnderwritingInterview
    {
        private readonly IUnderwritingWebServiceUrlProvider _underwritingWebServiceUrlProvider;
        private readonly IHttpClientService _httpClientService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public CompleteUnderwritingInterview(IUnderwritingWebServiceUrlProvider underwritingWebServiceUrlProvider, IHttpClientService httpClientService, ICurrentUserProvider currentUserProvider)
        {
            _underwritingWebServiceUrlProvider = underwritingWebServiceUrlProvider;
            _httpClientService = httpClientService;
            _currentUserProvider = currentUserProvider;
        }

        public bool Complete(string underwritingId, string concurrencyToken)
        {
            var urlString = _underwritingWebServiceUrlProvider.CompleteInterviewRequest(underwritingId);
            var currentUser = _currentUserProvider.GetForApplication();

            var postRequest = new PutOrPostRequest(new Uri(urlString), new CompleteInterviewRequest(currentUser.UserName))
                .WithEtag(concurrencyToken);
            
            return _httpClientService.PutAsync(postRequest).Result.StatusCode == HttpStatusCode.OK;
        }
    }
}

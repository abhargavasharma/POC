using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Service
{
    public interface ICreateUnderwritingInterview
    {
        ReadOnlyUnderwritingInterview CreateNewInterview(string templateName, string workflow, IEnumerable<string> benefitCodes );
    }

    public class CreateUnderwritingInterview : ICreateUnderwritingInterview
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUnderwritingWebServiceUrlProvider _underwritingWebServiceUrlProvider;
        private readonly ICreateInterviewRequestProvider _createInterviewRequestProvider;
        private readonly ICurrentUserProvider _currentUserProvider;

        public CreateUnderwritingInterview(IHttpClientService httpClientService,
            IUnderwritingWebServiceUrlProvider underwritingWebServiceUrlProvider,
            ICreateInterviewRequestProvider createInterviewRequestProvider,
            ICurrentUserProvider currentUserProvider)
        {
            _httpClientService = httpClientService;
            _underwritingWebServiceUrlProvider = underwritingWebServiceUrlProvider;
            _createInterviewRequestProvider = createInterviewRequestProvider;
            _currentUserProvider = currentUserProvider;
        }

        public ReadOnlyUnderwritingInterview CreateNewInterview(string templateName, string workflow, IEnumerable<string> benefitCodes)
        {
            var request = _createInterviewRequestProvider.Create(templateName, workflow, benefitCodes, _currentUserProvider.GetForApplication().UserName);

            var urlString = _underwritingWebServiceUrlProvider.InitiateUnderwritingUrl();

            var postRequest = new PutOrPostRequest(new Uri(urlString), request);

            var interview = _httpClientService.PostAsync<UnderwritingInterview>(postRequest).Result;

            return new ReadOnlyUnderwritingInterview(interview);
        }
    }
}

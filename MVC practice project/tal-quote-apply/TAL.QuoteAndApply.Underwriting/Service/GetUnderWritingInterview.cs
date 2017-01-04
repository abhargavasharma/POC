using System;
using System.Net;
using System.Net.Http;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Underwriting.Models.Converters;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Event;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Service
{
    public interface IGetUnderwritingInterview
    {
        ReadOnlyUnderwritingInterview GetInterview<TObserver>(string underwritingId, string concurrencyToken,
            ISubject<TObserver> benefitResponsesChangeSubject);
        ReadOnlyUnderwritingInterview GetInterviewWithoutSyncing(string underwritingId, string concurrencyToken);

        ReadOnlyUnderwritingInterview GetInterview<TObserver>(string underwritingId, ISubject<TObserver> benefitResponsesChangeSubject);

        bool InterviewExists(string underwritingId);
    }

    public class GetUnderwritingInterview : IGetUnderwritingInterview
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUnderwritingWebServiceUrlProvider _underwritingWebServiceUrlProvider;
        private readonly IUnderwritingBenefitResponsesChangeParamConverter _underwritingBenefitResponsesChangeParamConverter;
        private readonly ICachingWrapper _cachingWrapper;

        public GetUnderwritingInterview(IUnderwritingWebServiceUrlProvider underwritingWebServiceUrlProvider,
            IHttpClientService httpClientService,
            IUnderwritingBenefitResponsesChangeParamConverter underwritingBenefitResponsesChangeParamConverter, ICachingWrapper cachingWrapper)
        {
            _underwritingWebServiceUrlProvider = underwritingWebServiceUrlProvider;
            _httpClientService = httpClientService;
            _underwritingBenefitResponsesChangeParamConverter = underwritingBenefitResponsesChangeParamConverter;
            _cachingWrapper = cachingWrapper;
        }

        public ReadOnlyUnderwritingInterview GetInterview<TObserver>(string underwritingId, string concurrencyToken, ISubject<TObserver> benefitResponsesChangeSubject)
        {
            var readOnlyInterview = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"InterviewId-{underwritingId}",
                () => GetInterviewNoCache(underwritingId, concurrencyToken, benefitResponsesChangeSubject));

            if (readOnlyInterview.ConcurrencyToken == concurrencyToken)
            {
                return readOnlyInterview;
            }

            return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"InterviewId-{underwritingId}",
                GetInterviewNoCache(underwritingId, concurrencyToken, benefitResponsesChangeSubject));
        }

        public ReadOnlyUnderwritingInterview GetInterviewWithoutSyncing(string underwritingId, string concurrencyToken)
        {
            return GetInterview(underwritingId, concurrencyToken, (ISubject<UnderwritingBenefitResponsesChangeParam>)null);
        }

        public ReadOnlyUnderwritingInterview GetInterviewNoCache<TObserver>(string underwritingId, string concurrencyToken, ISubject<TObserver> benefitResponsesChangeSubject)
        {
            var urlString = _underwritingWebServiceUrlProvider.GetInterviewUrl(underwritingId);

            var getRequest = new GetRequest(new Uri(urlString))
                .WithEtag(concurrencyToken);

            var interview = _httpClientService.GetAsync<UnderwritingInterview>(getRequest).Result;

            var readonlyInterview = new ReadOnlyUnderwritingInterview(interview);

            benefitResponsesChangeSubject?.Notify(new ChangeEnvelope(_underwritingBenefitResponsesChangeParamConverter.CreateFrom(readonlyInterview)));

            return readonlyInterview;
        }

        public ReadOnlyUnderwritingInterview GetInterview<TObserver>(string underwritingId, ISubject<TObserver> benefitResponsesChangeSubject)
        {
            return GetInterview(underwritingId, null, benefitResponsesChangeSubject);
        }

        public bool InterviewExists(string underwritingId)
        {
            var urlString = _underwritingWebServiceUrlProvider.GetInterviewUrl(underwritingId);

            var getRequest = new GetRequest(new Uri(urlString))
                .WithEtag(null);

            return _httpClientService.GetAsync(getRequest).Result.StatusCode != HttpStatusCode.NotFound;
        }
    }

}
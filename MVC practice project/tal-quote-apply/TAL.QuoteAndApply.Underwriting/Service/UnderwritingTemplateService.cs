using System;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Underwriting.Configuration;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Service
{
    public interface IUnderwritingTemplateService
    {
        ReadOnlyTemplateInformation GetTemplateInformation(string templateName);
    }

    public class UnderwritingTemplateService : IUnderwritingTemplateService
    {
        private readonly IUnderwritingWebServiceUrlProvider _underwritingWebServiceUrlProvider;
        private readonly IHttpClientService _httpClientService;

        public UnderwritingTemplateService(IUnderwritingWebServiceUrlProvider underwritingWebServiceUrlProvider,
            IHttpClientService httpClientService)
        {
            _underwritingWebServiceUrlProvider = underwritingWebServiceUrlProvider;
            _httpClientService = httpClientService;
        }

        public ReadOnlyTemplateInformation GetTemplateInformation(string templateName)
        {
            var urlString = _underwritingWebServiceUrlProvider.GetTemplateInfoUrl(templateName);

            var templateInformation = _httpClientService.GetAsync<TemplateInformation>(new GetRequest(new Uri(urlString))).Result;
            return new ReadOnlyTemplateInformation(templateInformation);
        }
    }
}
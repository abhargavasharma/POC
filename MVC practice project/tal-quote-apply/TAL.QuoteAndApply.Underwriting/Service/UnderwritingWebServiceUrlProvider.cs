using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Underwriting.Configuration;

namespace TAL.QuoteAndApply.Underwriting.Service
{
    public interface IUnderwritingWebServiceUrlProvider
    {
        string InitiateUnderwritingUrl();
        string GetInterviewUrl(string underwritingId);
        string GetTemplateQuestionUrl(string templateType, string questionId);
        string CompleteInterviewRequest(string underwritingId);
        string GetTemplateInfoUrl(string templateTypeName);
        Uri GetAliveUrl();
    }

    public class UnderwritingWebServiceUrlProvider : IUnderwritingWebServiceUrlProvider
    {
        internal static class ServiceUrl
        {
            public const string InterviewRoot = "api/interview";
            public const string TemplateRoot = "api/template";
            public const string TemplateInformation = "api/template/type";
            public const string AliveRoot = "api/alive";
        }

        private readonly IUnderwritingConfigurationProvider _configurationService;

        public UnderwritingWebServiceUrlProvider(IUnderwritingConfigurationProvider configurationService)
        {
            _configurationService = configurationService;
        }


        public string InitiateUnderwritingUrl()
        {
            var newUri = new Uri(new Uri(_configurationService.UnderwritingApiBaseUrl), ServiceUrl.InterviewRoot);
            return newUri.ToString();
        }

        private string GetUrlWithHost(string url)
        {
            var newUri = new Uri(new Uri(_configurationService.UnderwritingApiBaseUrl), url);
            return newUri.ToString();
        }

        public string GetInterviewUrl(string underwritingId)
        {
            return GetUrlWithHost(String.Format("{0}/{1}", ServiceUrl.InterviewRoot, underwritingId));
        }

        public string GetTemplateQuestionUrl(string templateType, string questionId)
        {
            return GetUrlWithHost(String.Format("{0}/{1}/question/{2}", ServiceUrl.TemplateRoot, templateType,
                questionId));
        }

        public string CompleteInterviewRequest(string underwritingId)
        {
            return GetUrlWithHost(String.Format("{0}/{1}/complete", ServiceUrl.InterviewRoot, underwritingId));
        }

        public string GetTemplateInfoUrl(string templateTypeName)
        {
            return GetUrlWithHost(String.Format("{0}/{1}", ServiceUrl.TemplateInformation, templateTypeName));
        }

        public Uri GetAliveUrl()
        {
            return new Uri(GetUrlWithHost(ServiceUrl.AliveRoot));
        }
    }
}

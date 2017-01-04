using System;
using TAL.Enterprise.Superstream.Acord.Binary;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Http.Exceptions;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Payment.Configuration;

namespace TAL.QuoteAndApply.Payment.Service.SuperFund
{
    public class SuperFundLookupParams
    {
        public string USI { get; set; }
        public string ABN { get; set; }
        public string Organization { get; set; }
        public string Product { get; set; }

        public SuperFundLookupParams()
        {
            USI = "";
            ABN = "";
            Organization = "";
            Product = "";
        }
    }

    public interface IHttpSuperFundLookupService
    {
        InvestmentInquiryNotify_Type Search(SuperFundLookupParams param);
    }

    public class HttpSuperFundLookupService : IHttpSuperFundLookupService
    {
        private readonly IHttpClientService _clientService;
        private readonly ISuperannuationConfigurationProvider _configuration;
        private readonly ILoggingService _loggingService;

        public HttpSuperFundLookupService(IHttpClientService clientService, ISuperannuationConfigurationProvider configuration, ILoggingService loggingService)
        {
            _clientService = clientService;
            _configuration = configuration;
            _loggingService = loggingService;
        }

        public InvestmentInquiryNotify_Type Search(SuperFundLookupParams param)
        {
            var queryString = string.Format("usi={0}&abn={1}&product={2}&organization={3}", param.USI, param.ABN, param.Product, param.Organization);
            var endPointUrl = _configuration.SuperannuationServiceBaseUrl + "/api/fund?" + queryString;

            var getRequest = new GetRequest(new Uri(endPointUrl))
                .WithSourceType(SourceType.Xml);

            try
            {
                return _clientService.GetAsync<InvestmentInquiryNotify_Type>(getRequest).Result;
            }
            catch (Exception ex)
            {
                //TODO: for now the service throws 404 when there are no results.
                //we will treat this as empty result instead:
                if (ex.InnerException != null && ex.InnerException.Message.Contains("404"))
                {
                    return null;
                }

                var exception = new ThirdPartyServiceException("Exception during superfund search", ex);
                _loggingService.Error(exception);
                throw exception;
            }
        }
    }
}

using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Configuration;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Infrastructure.Mail;
using TAL.QuoteAndApply.Infrastructure.Resource;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Infrastructure.Url;

namespace TAL.QuoteAndApply.Infrastructure.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.Add<LoggingService>()
                .Add<HttpClientService>()
                .Add<HttpResponseMessageSerializer>()
                .Add<HttpRequestMessageSerializer>()
                .Add<MimeTypeProvider>()
                .Add<DateTimeProvider>()
                .Add<CachingService>()
                .Add<CachingWrapper>()
                .Add<HttpContextProvider>()
                .Add<CurrentUrlProvider>()
                .Add<EndRequestOperationCollection>()
                .Add<UrlUtilities>()
                .Add<PasEncryptionConfigurationProvider>()
                .Add<PasEncryptionHttpService>()
                .Add<SmtpClientFactory>()
                .Add<SmtpService>()
                .Add<MailMessageBuilder>()
                .Add<ResourceFileReader>()
                .Add<Features.Features>()
                .Add<RestProxyConfigurationProvider>()
                ;
        }
    }
}

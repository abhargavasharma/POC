using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Notifications.Configuration;
using TAL.QuoteAndApply.Notifications.Models;
using TAL.QuoteAndApply.Notifications.Service;

namespace TAL.QuoteAndApply.Notifications.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {

            mapper.WhenRequesting<IEmailQuoteViewModelProvider>().ProvideImplementationOf<EmailQuoteViewModelProvider>();
            mapper
                .Add<EmailQuoteService>()
                .Add<EmailErrorNotificationService>()
                .Add<EmailConfigurationProvider>()
                .Add<EmailTemplateConstantsProvider>();
        }
    }
}

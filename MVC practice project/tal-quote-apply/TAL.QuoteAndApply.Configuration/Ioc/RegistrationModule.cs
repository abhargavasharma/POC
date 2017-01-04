using TAL.QuoteAndApply.Configuration.Data;
using TAL.QuoteAndApply.Configuration.Service;
using TAL.QuoteAndApply.Infrastructure.Ioc;

namespace TAL.QuoteAndApply.Configuration.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.WhenRequesting<IApplicationConfigurationSettings>()
                .ProvideImplementationOf<ApplicationConfigurationSettings>();
            mapper.WhenRequesting<IConfigurationService>()
                .ProvideImplementationOf<ConfigurationService>();
            mapper.WhenRequesting<IConfigurationItemRepository>()
                .ProvideImplementationOf<ConfigurationItemRepository>();
        }
    }
}

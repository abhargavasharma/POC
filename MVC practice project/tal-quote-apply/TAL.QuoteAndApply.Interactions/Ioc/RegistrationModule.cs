using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Interactions.Configuration;
using TAL.QuoteAndApply.Interactions.Data;
using TAL.QuoteAndApply.Interactions.Service;

namespace TAL.QuoteAndApply.Interactions.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.Add<InteractionsService>();

            mapper.WhenRequesting<IPolicyInteractionDtoRepository>().ProvideImplementationOf<PolicyInteractionDtoRepository>();
            mapper.WhenRequesting<IInteractionsConfigurationProvider>().ProvideImplementationOf<InteractionsConfigurationProvider>();
        }
    }
}

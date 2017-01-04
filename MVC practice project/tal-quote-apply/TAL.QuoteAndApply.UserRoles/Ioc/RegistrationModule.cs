using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.UserRoles.Configuration;
using TAL.QuoteAndApply.UserRoles.Customer;
using TAL.QuoteAndApply.UserRoles.Services;

namespace TAL.QuoteAndApply.UserRoles.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            AddSalesPortalDependencies(mapper);
            AddCustomerDependencies(mapper);
        }

        private void AddCustomerDependencies(ISimpleDependencyMapper mapper)
        {
            mapper.Add<OktaConfigurationProvider>();
            mapper.WhenRequesting<ICustomerAuthenticationService>()
                .ProvideImplementationOf<OktaCustomerAuthenticationService>();
        }

        private void AddSalesPortalDependencies(ISimpleDependencyMapper mapper)
        {
            mapper
                .Add<UserPrincipalService>()
                .Add<AuthenticationService>()
                .Add<OktaAuthenticationResultFactory>();
        }
    }
}

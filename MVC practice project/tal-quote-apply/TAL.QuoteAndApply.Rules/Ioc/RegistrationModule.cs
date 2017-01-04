using TAL.QuoteAndApply.Infrastructure.Ioc;

namespace TAL.QuoteAndApply.Rules.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.Add<GenericRuleFactory>();
        }
    }
}

using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Rules;
using TAL.QuoteAndApply.Product.Service;

namespace TAL.QuoteAndApply.Product.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.Add<RuleFactory>()
                .Add<PlanDefinitionProvider>()
                .Add<CoverAmountService>()
                .Add<NameLookupService>()
                .Add<PlanDefaultsProvider>()
                .Add<PlanDefaultsBuilder>()
                .Add<PolicyDefaultsProvider>()
                .Add<CoverDefinitionProvider>()
                .Add<MaxCoverAmountForAgeProvider>()
                .Add<MaxCoverAmountPercentageOfIncomeProvider>()
                .Add<PlanMaxEntryAgeNextBirthdayProvider>()
                .Add<ProductDefinitionBuilder>();
        }
    }
}

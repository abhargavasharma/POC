using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Data;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Leads.CallbackService;
using TAL.QuoteAndApply.Party.Leads.LeadsService;
using TAL.QuoteAndApply.Party.Postcode;
using TAL.QuoteAndApply.Party.Rules;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Party.Validation;

namespace TAL.QuoteAndApply.Party.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.Add<PartyRulesService>()
                .Add<PartyRuleFactory>()
                .Add<PartyService>()
                .Add<LeadConfigurationProvider>()
                .Add<ChatConfigurationProvider>()
                .Add<PartyToLeadsMessageConverter>()
                .Add<HttpLeadsService>()
                .Add<SyncLeadService>()
                .Add<PartyConsentService>()
                .Add<AdobeServiceResultConverter>()
                .Add<RequestCallbackService>()
                .Add<GetLeadService>()
                .Add<AustralianStateFactory>()
                .Add<AustralianStateProvider>();
                
            mapper.WhenRequesting<IPartyDtoRepository>().ProvideImplementationOf<PartyDtoRepository>();
            mapper.WhenRequesting<IPartyConfigurationProvider>().ProvideImplementationOf<PartyConfigurationProvider>();
            mapper.WhenRequesting<IPartyConsentDtoRepository>().ProvideImplementationOf<PartyConsentDtoRepository>();
        }
    }
}

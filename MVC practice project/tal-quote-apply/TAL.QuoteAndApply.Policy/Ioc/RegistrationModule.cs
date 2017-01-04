using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models.Converters;
using TAL.QuoteAndApply.Policy.Rules.Beneficiary;
using TAL.QuoteAndApply.Policy.Rules.Plan;
using TAL.QuoteAndApply.Policy.Rules.Cover;
using TAL.QuoteAndApply.Policy.Rules.Retrieval;
using TAL.QuoteAndApply.Policy.Rules.Risk;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.AccessControl;
using TAL.QuoteAndApply.Policy.Service.MarketingStatus;
using TAL.QuoteAndApply.Policy.Service.RaisePolicy;

namespace TAL.QuoteAndApply.Policy.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.WhenRequesting<IRiskDtoRepository>().ProvideImplementationOf<RiskDtoRepository>();
            mapper.WhenRequesting<IPolicyConfigurationProvider>().ProvideImplementationOf<PolicyConfigurationProvider>();
            mapper.WhenRequesting<IPolicyDtoRepository>().ProvideImplementationOf<PolicyDtoRepository>();
            mapper.WhenRequesting<IBrandDtoRepository>().ProvideImplementationOf<BrandDtoRepository>();
            mapper.WhenRequesting<ICreatePolicyService>().ProvideImplementationOf<CreatePolicyService>();
            mapper.WhenRequesting<IPlanDtoRepository>().ProvideImplementationOf<PlanDtoRepository>();
            mapper.WhenRequesting<ICoverDtoRepository>().ProvideImplementationOf<CoverDtoRepository>();
            mapper.WhenRequesting<IOptionDtoRepository>().ProvideImplementationOf<OptionDtoRepository>();
			mapper.WhenRequesting<IBeneficiaryDtoRepository>().ProvideImplementationOf<BeneficiaryDtoRepository>();
			mapper.WhenRequesting<INoteDtoRepository>().ProvideImplementationOf<NoteDtoRepository>();
            mapper.WhenRequesting<IPolicyAnalyticsIdentifierDtoRepository>().ProvideImplementationOf<PolicyAnalyticsIdentifierDtoRepository>();
            mapper.WhenRequesting<IPolicyOwnerDtoRepository>().ProvideImplementationOf<PolicyOwnerDtoRepository>();
            mapper.WhenRequesting<IBeneficiaryRelationshipDtoRepository>().ProvideImplementationOf<BeneficiaryRelationshipDtoRepository>();
			mapper.WhenRequesting<IPolicySearchDtoRepository>().ProvideImplementationOf<PolicySearchDtoRepository>();
            mapper.WhenRequesting<ICoverLoadingDtoRepository>().ProvideImplementationOf<CoverLoadingDtoRepository>();
            mapper.WhenRequesting<ICoverExclusionDtoRepository>().ProvideImplementationOf<CoverExclusionDtoRepository>();
            mapper.WhenRequesting<IRaisePolicySubmissionAuditDtoRepository>().ProvideImplementationOf<RaisePolicySubmissionAuditDtoRepository>();
            mapper.WhenRequesting<IPolicyAccessControlDtoRepository>().ProvideImplementationOf<PolicyAccessControlDtoRepository>();
            mapper.WhenRequesting<IRiskOccupationDtoRepository>().ProvideImplementationOf<RiskOccupationDtoRepository>();
            mapper.WhenRequesting<IRiskOccupationService>().ProvideImplementationOf<RiskOccupationService>();
            mapper.WhenRequesting<IAgentDashboardSearchService>().ProvideImplementationOf<AgentDashboardSearchService>();
            mapper.WhenRequesting<IAgentDashboardSearchDtoRepository>().ProvideImplementationOf<AgentDashboardSearchDtoRepository>();
            mapper.WhenRequesting<IRiskMarketingStatusDtoRepository>().ProvideImplementationOf<RiskMarketingStatusDtoRepository>();
            mapper.WhenRequesting<IPlanMarketingStatusDtoRepository>().ProvideImplementationOf<PlanMarketingStatusDtoRepository>();
            mapper.WhenRequesting<ICoverMarketingStatusDtoRepository>().ProvideImplementationOf<CoverMarketingStatusDtoRepository>();
            mapper.WhenRequesting<IOrganisationDtoRepository>().ProvideImplementationOf<OrganisationDtoRepository>();

            mapper.Add<PolicyService>()
                .Add<PlanService>()
                .Add<RiskService>()
                .Add<RiskRulesService>()
				.Add<CoverService>()
                .Add<RiskRulesFactory>()
				.Add<OptionService>()
                .Add<RiskRulesService>()
                .Add<RiskRulesFactory>()
                .Add<QuoteReferenceGenerationService>()
                .Add<NoteService>()
                .Add<CoverRulesFactory>()
                .Add<PolicyAnalyticsIdentifierService>()
                .Add<PolicyOwnerService>()
                .Add<PolicySearchService>()
                .Add<CoverLoadingService>()
                .Add<CoverExclusionsService>()
                .Add<RaisePolicySubmissionAuditService>()
                .Add<PolicyRetrievalRulesFactory>()
                .Add<RiskMarketingStatusService>()
                .Add<CoverMarketingStatusService>()
                .Add<PlanMarketingStatusService>()
                .Add<RiskMarketingStatusProvider>()
                .Add<CoverMarketingStatusProvider>()
                .Add<PlanMarketingStatusProvider>();

            mapper.WhenRequesting<IResidencyStatusConverter>().ProvideImplementationOf<RiskEnumConverter>();

            mapper.WhenRequesting<IPaymentOptionsAvailabilityRule>()
                .ProvideImplementationOf<CreditCardPaymentAvailabilityRule>();
            mapper.WhenRequesting<IPaymentOptionsAvailabilityRule>()
                .ProvideImplementationOf<DirectDebitPaymentAvailabilityRule>();

            mapper.WhenRequesting<IPaymentOptionsAvailabilityRule>()
                .ProvideImplementationOf<SelfManagedSuperFundPaymentAvailabilityRule>();

            mapper.WhenRequesting<IPaymentOptionsAvailabilityRule>()
                .ProvideImplementationOf<SuperAnnuationPaymentAvailabilityRule>();

            mapper.Add<PlanRulesFactory>();

            mapper.Add<PaymentOptionsAvailabilityProvider>();
            mapper.Add<BenefeciaryRuleFactory>();

            mapper.WhenRequesting<IHttpRaisePolicyService>().ProvideImplementationOf<HttpRaisePolicyService>();
            mapper.WhenRequesting<IRaisePolicyConfigurationProvider>().ProvideImplementationOf<RaisePolicyConfigurationProvider>();
            mapper.WhenRequesting<IRaisePolicyDefinitionBuilder>().ProvideImplementationOf<RaisePolicyDefinitionBuilder>();

            mapper.Add<ReferralService>();
            mapper.Add<ReferralDtoRepository>();
            mapper.Add<AccessControlTypeProvider>();
            mapper.Add<PolicyAccessControlCreationService>();
            mapper.Add<PolicyAccessControlService>();
            mapper.Add<PolicyEventsRegistration>();
            mapper.Add<RaisePolicyFeedbackResponseConverter>();

            mapper.WhenRequesting<ICoverChangeSubject>().ProvideImplementationOf<CoverChangeSubject>().InSingletonScope(); 
            mapper.WhenRequesting<IPlanChangeSubject>().ProvideImplementationOf<PlanChangeSubject>().InSingletonScope(); 
            mapper.WhenRequesting<IRiskChangeSubject>().ProvideImplementationOf<RiskChangeSubject>().InSingletonScope(); 
            mapper.WhenRequesting<IOptionChangeSubject>().ProvideImplementationOf<OptionChangeSubject>().InSingletonScope(); 
            mapper.WhenRequesting<ICoverLoadingChangeSubject>().ProvideImplementationOf<CoverLoadingChangeSubject>().InSingletonScope(); 
            mapper.WhenRequesting<IRiskOccupationChangeSubject>().ProvideImplementationOf<RiskOccupationChangeSubject>().InSingletonScope();
            mapper.WhenRequesting<ICoverExclusionChangeSubject>().ProvideImplementationOf<CoverExclusionChangeSubject>().InSingletonScope();

            mapper.WhenRequesting<ICoverChangeObserver>().ProvideImplementationOf<PolicyAccessControlObserver>();
            mapper.WhenRequesting<ICoverLoadingChangeObserver>().ProvideImplementationOf<PolicyAccessControlObserver>();
            mapper.WhenRequesting<IRiskChangeObserver>().ProvideImplementationOf<PolicyAccessControlObserver>();
            mapper.WhenRequesting<IPlanChangeObserver>().ProvideImplementationOf<PolicyAccessControlObserver>();
            mapper.WhenRequesting<IOptionChangeObserver>().ProvideImplementationOf<PolicyAccessControlObserver>();
            mapper.WhenRequesting<IRiskOccupationChangeObserver>().ProvideImplementationOf<PolicyAccessControlObserver>();
            mapper.WhenRequesting<ICoverExclusionChangeObserver>().ProvideImplementationOf<PolicyAccessControlObserver>();
        }
    }
}

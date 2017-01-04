using System.Web.Http;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using TAL.QuoteAndApply.Notifications.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Configuration;
using TAL.QuoteAndApply.SalesPortal.Web.Factories;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Plan;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Referral;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Referral;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard;
using TAL.QuoteAndApply.ServiceLayer.User;
using TAL.QuoteAndApply.UserRoles.Configuration;
using TAL.QuoteAndApply.Web.Shared.ErrorMessages;
using TAL.QuoteAndApply.Web.Shared.Ioc;
using PolicyInteractionsResultConverter = TAL.QuoteAndApply.SalesPortal.Web.Services.Converters.PolicyInteractionsResultConverter;
using PolicyNotesResultConverter = TAL.QuoteAndApply.SalesPortal.Web.Services.Converters.PolicyNotesResultConverter;

namespace TAL.QuoteAndApply.SalesPortal.Web
{
    public static class IocConfig
    {
        private static readonly BaseIocConfig BaseIocConfig = new BaseIocConfig();

        public static void RegisterForWeb(HttpConfiguration config)
        {
            BaseIocConfig.RegisterForWeb(config, RegisterSalesPortal);
        }
        
        public static void RegisterSalesPortal(ContainerBuilder builder)
        {
            //UserRoles is not referenced from the service layer, it is a sales portal only thing, so register is seperatly 
            BaseIocConfig.RegisterModuleMappings(builder, typeof(UserRoles.Ioc.RegistrationModule), "TAL.QuoteAndApply.UserRoles");

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly);
            
            RegisterUserAndSessionDependencies(builder);

            builder.RegisterType<CommonProductErrorMessagesService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CommonPlanErrorMessageService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<UrlService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PlanModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SalesPortalConfiguration>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<BeneficiaryDetailsRequestConverter>().SingleInstance().AsImplementedInterfaces();
            builder.RegisterType<PlanStateParamFactory>().SingleInstance().AsImplementedInterfaces();
            builder.RegisterType<PlanUpdateResponseConverter>().SingleInstance().AsImplementedInterfaces();
            builder.RegisterType<PlanDetailsService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PlanDetailsService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PaymentOptionService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<EditPolicyPermissionsService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<GetReferralsService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<GetUnderwritersService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<RoleDashboardRedirectService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<AssignReferralService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PartyConsentProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<GetAgentDashboardQuotesService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PlanOverviewResultProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyProgressProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SalesPortalPolicyRetrievalService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyCorrespondenceService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyCorrespondenceEmailService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SalesPortalSummaryProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SalesPortalCurrentProductBrandProvider>().As<ICurrentProductBrandProvider>().InstancePerLifetimeScope();
			builder.RegisterType<UnderwritingViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<BrandSettingsProvider>().As<IBrandSettingsProvider>().InstancePerLifetimeScope();
			builder.RegisterType<UserRolesConfigurationProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<RaisePolicyOwnershipValidationService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<BrandAuthorizationService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<BrandExternalRefSettings>().As<IBrandExternalRefSettings>().InstancePerLifetimeScope();
            builder.RegisterType<EmailTemplateConstantsProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<SalesPortalUiBrandingHelper>().InstancePerLifetimeScope().AsImplementedInterfaces();
            RegisterConverters(builder);
        }

        private static void RegisterUserAndSessionDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<SalesPortalCurrentUser>().As<IApplicationCurrentUser>().InstancePerLifetimeScope();
            builder.RegisterType<SalesPortalSessionConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SalesPortalSessionContext>().InstancePerLifetimeScope().AsImplementedInterfaces();
        }

        private static void RegisterConverters(ContainerBuilder builder)
        {
            builder.RegisterType<RetrievePolicyViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PersonalDetailsRequestConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<RatingFactorsRequestConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CreateClientRequestConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PlanDetailResponseConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SuperFundSearchResultConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SuperFundPaymentViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CreditCardPaymentViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<DirectDebitPaymentViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SelfManagedSuperFundPaymentViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PaymentOptionsViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyStatusViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SearchQuotesClientsAndProspectsRequestConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SearchQuotesClientsAndProspectsResultConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyPremiumFrequencyViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<ExternalCustomerReferenceViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<RaisePaymentConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<RiskPremiumSummaryViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PlanResponseToStateParamConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyInteractionsRequestConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyInteractionsResultConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<ReferralDetailsResultConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyNotesRequestConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyNotesResultConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PartyConsentRequestConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PartyConsentResultConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PartyConsentParamConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<AgentDashboardResultConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<AgentDashboardQuoteResultConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyProgressViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SetInforceResponseConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyCorrespondenceResultConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyOwnerDetailsConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
          
        }
    }
}
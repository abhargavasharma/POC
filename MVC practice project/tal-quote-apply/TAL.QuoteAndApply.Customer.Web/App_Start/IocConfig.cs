using System.Web.Http;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Configuration;
using TAL.QuoteAndApply.Customer.Web.Converters;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.Customer.Web.Services.Converters;
using TAL.QuoteAndApply.Customer.Web.User;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.User;
using TAL.QuoteAndApply.Web.Shared.Ioc;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web
{
    public static class IocConfig
    {
        private static readonly BaseIocConfig BaseIocConfig = new BaseIocConfig();

        public static void RegisterForWeb(HttpConfiguration config)
        {
            BaseIocConfig.RegisterForWeb(config, RegisterInternalModules);

        }

        public static void RegisterInternalModules(ContainerBuilder builder)
        {
            BaseIocConfig.RegisterModuleMappings(builder, typeof(Analytics.Ioc.RegistrationModule), "TAL.QuoteAndApply.Analytics");

            builder.RegisterControllers(typeof(CustomerApplication).Assembly);
            builder.RegisterApiControllers(typeof(CustomerApplication).Assembly);

            builder.RegisterType<CustomerPortalCurrentUser>().As<IApplicationCurrentUser>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerProductErrorMessageService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CustomerPlanErrorMessageService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<QuoteSessionContext>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SecurityService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<BeneficiaryDetailsService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<BeneficiaryValidationServiceAdapter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PaymentOptionService>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<QuoteParamConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyOverviewProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<ProductRulesService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CustomerPurchaseDetailsService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CustomerSubmissionService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CustomerPurchaseValidationService>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<PolicyInitialisationMetadataProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<RiskPersonalDetailsProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<PolicyDeclarationService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PaymentOptionService>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<PlanDetailsService>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<PlanOptionParamConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<PlanStateParamFactory>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<CustomerPolicyStatusService>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<CustomerReviewValidationService>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<CustomerPolicyRetrievalService>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<CustomerPortalConfiguration>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<GoogleCaptchaService>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CaptchaConfigurationProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CaclculatorConfigurationProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<BaseCustomerControllerHelper>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<AngularConfigurationProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<BrandSettingsProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
			builder.RegisterType<CustomerPortalCurrentProductBrandProvider>().As<ICurrentProductBrandProvider>().InstancePerLifetimeScope();

            RegisterConverters(builder);
            RegisterFilters(builder);
        }

        private static void RegisterConverters(ContainerBuilder builder)
        {
            builder.RegisterType<QuoteParamConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<BeneficiaryDetailsRequestConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CreditCardPaymentViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<UnderwritingViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<CustomerPolicyRiskService>().InstancePerLifetimeScope().AsImplementedInterfaces();			
            builder.RegisterType<PolicyInitialisationMetadataProvider>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PolicyNoteRequestConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PersonalDetailsResultConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<PaymentOptionsViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<DirectDebitPaymentViewModelConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<SaveCustomerRequestConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
            builder.RegisterType<UnderwritingQuestionChoiceResponseConverter>().InstancePerLifetimeScope().AsImplementedInterfaces();
        }

        private static void RegisterFilters(ContainerBuilder builder)
        {
            builder.Register(c => new WebApiQuoteSessionRequiredAttribute())
                .AsWebApiAuthorizationFilterFor<ApiController>().InstancePerLifetimeScope();
        }

    }
}
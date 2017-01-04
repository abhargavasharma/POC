using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Payment.Configuration;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Rules;
using TAL.QuoteAndApply.Payment.Service;
using TAL.QuoteAndApply.Payment.Service.SuperFund;
using TAL.QuoteAndApply.Payment.Service.TFN;

namespace TAL.QuoteAndApply.Payment.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.WhenRequesting<ICreditCardPaymentRepository>().ProvideImplementationOf<CreditCardPaymentRepository>();
            mapper.WhenRequesting<IDirectDebitPaymentRepository>().ProvideImplementationOf<DirectDebitPaymentRepository>();
            mapper.WhenRequesting<ISelfManagedSuperFundPaymentRepository>().ProvideImplementationOf<SelfManagedSuperFundPaymentRepository>();
            mapper.WhenRequesting<ISuperAnnuationPaymentRepository>().ProvideImplementationOf<SuperAnnuationPaymentRepository>();
            mapper.WhenRequesting<IPolicyPaymentRepository>().ProvideImplementationOf<PolicyPaymentRepository>();

            mapper.WhenRequesting<ICreditCardTokenisationServiceFactory>()
                .ProvideImplementationOf<CreditCardTokenisationServiceFactory>();

            mapper.WhenRequesting<ICreditCardPaymentOptionService>().ProvideImplementationOf<CreditCardPaymentOptionService>();
            mapper.WhenRequesting<ISuperAnnuationPaymentOptionService>().ProvideImplementationOf<SuperAnnuationPaymentOptionService>();
            mapper.WhenRequesting<IDirectDebitPaymentOptionService>().ProvideImplementationOf<DirectDebitPaymentOptionService>();
            mapper.WhenRequesting<ISelfManagedSuperFundPaymentOptionService>().ProvideImplementationOf<SelfManagedSuperFundPaymentOptionService>();
            mapper.WhenRequesting<IPolicyPaymentService>().ProvideImplementationOf<PolicyPaymentService>();

            mapper.WhenRequesting<IPaymentConfigurationProvider>()
                .ProvideImplementationOf<PaymentConfigurationProvider>();
            mapper.WhenRequesting<ISuperannuationConfigurationProvider>().ProvideImplementationOf<SuperannuationConfigurationProvider>();
            mapper.WhenRequesting<IHttpSuperFundLookupService>().ProvideImplementationOf<HttpSuperFundLookupService>();
            mapper.WhenRequesting<ISuperFundConverter>().ProvideImplementationOf<SuperFundConverter>();
            mapper.WhenRequesting<ISuperAnnuationFundProvider>().ProvideImplementationOf<SuperAnnuationFundProvider>();

            mapper.WhenRequesting<IPaymentOptionsRuleFactory>().ProvideImplementationOf<PaymentOptionsRuleFactory>();
            mapper.WhenRequesting<IPaymentRulesService>().ProvideImplementationOf<PaymentRulesService>();

            mapper.WhenRequesting<ITaxFileNumberEncyptionService>()
                .ProvideImplementationOf<TaxFileNumberEncyptionService>();
        }
    }
}

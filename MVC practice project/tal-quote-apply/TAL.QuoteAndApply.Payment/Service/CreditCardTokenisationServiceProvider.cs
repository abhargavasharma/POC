using TalDirect.TokenisationClient;
using TAL.QuoteAndApply.Payment.Configuration;

namespace TAL.QuoteAndApply.Payment.Service
{
    public interface ICreditCardTokenisationServiceFactory
    {
        ICreditCardTokenisationService Build();
    }

    public class CreditCardTokenisationServiceFactory : ICreditCardTokenisationServiceFactory
    {
        private readonly IPaymentConfigurationProvider _paymentConfigurationProvider;

        public CreditCardTokenisationServiceFactory(IPaymentConfigurationProvider paymentConfigurationProvider)
        {
            _paymentConfigurationProvider = paymentConfigurationProvider;
        }

        public ICreditCardTokenisationService Build()
        {
            return new CreditCardTokenisationService(_paymentConfigurationProvider.TokenisationBaseUrl,
                _paymentConfigurationProvider.TokenisationIdentityUser,
                _paymentConfigurationProvider.TokenisationThumbPrint,
                _paymentConfigurationProvider.TokenisationSourceSystemCode);
        }
    }

}

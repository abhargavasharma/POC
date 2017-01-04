using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.Services.Converters
{
    public interface ISaveCustomerRequestConverter
    {
        SaveCustomerParam From(int riskId, SaveCustomerRequest saveCustomerRequest);
        CreateLoginParam From(int riskId, CreateLoginRequest createLoginRequest);
        SaveCustomerRequest From(SaveCustomerParam saveCustomerParam);
        SaveCustomerParam From(ChatRequestCallbackRequest request);
    }

    public class SaveCustomerRequestConverter : ISaveCustomerRequestConverter
    {
        public SaveCustomerParam From(int riskId, SaveCustomerRequest saveCustomerRequest)
        {
            return new SaveCustomerParam()
            {
                RiskId = riskId,
                FirstName = saveCustomerRequest.FirstName,
                LastName = saveCustomerRequest.LastName,
                PhoneNumber = saveCustomerRequest.PhoneNumber,
                EmailAddress = saveCustomerRequest.EmailAddress,
                ExpressConsent = saveCustomerRequest.ExpressConsent
            };
        }
        public CreateLoginParam From(int riskId, CreateLoginRequest createLoginRequest)
        {
            return new CreateLoginParam()
            {
                RiskId = riskId,
                Password = createLoginRequest.Password

            };
        }
        public SaveCustomerRequest From(SaveCustomerParam saveCustomerParam)
        {
            return new SaveCustomerRequest
            {
                FirstName = saveCustomerParam.FirstName,
                LastName = saveCustomerParam.LastName,
                PhoneNumber = saveCustomerParam.PhoneNumber,
                EmailAddress = saveCustomerParam.EmailAddress
            };
        }

        public SaveCustomerParam From(ChatRequestCallbackRequest request)
        {
            return new SaveCustomerParam()
            {
                RiskId = request.RiskId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                EmailAddress = request.EmailAddress
            };
        }
    }
}
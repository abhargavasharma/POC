using System.Linq;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public interface ICustomerPurchaseValidationService
    {
        CustomerPurchaseValidationModel ValidatePurchaseForSave(PurchaseRequest purchaseRequest);
    }
    

    public class CustomerPurchaseValidationService : ICustomerPurchaseValidationService
    {
        private readonly IBeneficiaryValidationServiceAdapter _beneficiaryValidationService;
        private readonly IBeneficiaryDetailsRequestConverter _modelMapper;

        public CustomerPurchaseValidationService(IBeneficiaryValidationServiceAdapter beneficiaryValidationService, IBeneficiaryDetailsRequestConverter modelMapper)
        {
            _beneficiaryValidationService = beneficiaryValidationService;
            _modelMapper = modelMapper;
        }

        public CustomerPurchaseValidationModel ValidatePurchaseForSave(PurchaseRequest purchaseRequest)
        {
            var result = new CustomerPurchaseValidationModel()
            {
                Beneficiaries = _beneficiaryValidationService.ValidateBeneficiariesForInForce(
                        purchaseRequest.Beneficiaries.Select(_modelMapper.From).ToArray(), purchaseRequest.RiskId).ToArray()
            };

            if (!purchaseRequest.PaymentOptions.IsCreditCardSelected &&
                    !purchaseRequest.PaymentOptions.IsDirectDebitSelected)
            {
                result.PaymentErrorMessage = "Payment Details are required";
            }

            if (purchaseRequest.PaymentOptions.CreditCardPayment == null &&
                purchaseRequest.PaymentOptions.DirectDebitPayment == null)
            {
                result.PaymentErrorMessage = "Payment Details are required";
            }

            if (purchaseRequest.PaymentOptions.IsCreditCardSelected &&
                purchaseRequest.PaymentOptions.IsDirectDebitSelected)
            {
                result.PaymentErrorMessage = "Multiple payments are not allowed";
            }

            return result;
        }
    }
}
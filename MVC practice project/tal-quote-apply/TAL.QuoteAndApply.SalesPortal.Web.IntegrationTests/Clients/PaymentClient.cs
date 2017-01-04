using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class PaymentClient : SalesPortalApiClient
    {
        public async Task<PaymentOptionsViewModel> GetAvailablePaymentOptionsAsync(string quoteReferenceNumber, int riskId)
        {
            var uri = Client.CreateUri("api/policy/{0}/payment/risk/{1}/paymentOptions", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get Available Payment Options");

            var response = await Client.GetAsync<PaymentOptionsViewModel>(uri, throwOnFailure: false);

            PerformanceTestTool.EndTransaction("API: Get Available Payment Options");

            PerformanceTestTool.LogInformation("API: Get Available Payment Options Done");

            return response;
        }

        public async Task<TResponse> PayViaDirectDebitAsync<TResponse>(string quoteReferenceNumber, int riskId, DirectDebitPaymentViewModel directDebitPaymentViewModel, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/policy/{0}/payment/risk/{1}/directdebit", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Post Direct Debit Payment");

            var response = await Client.PostAsync<DirectDebitPaymentViewModel, TResponse>(uri, directDebitPaymentViewModel, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Post Direct Debit Payment");

            PerformanceTestTool.LogInformation("API: Post Direct Debit Payment Done");

            return response;
        }

        public async Task<TResponse> PayViaCreditCardAsync<TResponse>(string quoteReferenceNumber, int riskId, CreditCardPaymentViewModel creditCardPaymentViewModel, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/policy/{0}/payment/risk/{1}/creditcard", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Post Credit Card Payment");

            var response = await Client.PostAsync<CreditCardPaymentViewModel, TResponse>(uri, creditCardPaymentViewModel, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Post Credit Cardt Payment");

            PerformanceTestTool.LogInformation("API: Post Credit Card Payment");

            return response;
        }

        public async Task<TResponse> PayViaSuperanuationAsync<TResponse>(string quoteReferenceNumber, int riskId, SuperFundPaymentViewModel superFundPaymentViewModel)
        {
            var uri = Client.CreateUri("api/policy/{0}/payment/risk/{1}/superannuation", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Post Super Fund Payment");

            var response = await Client.PostAsync<SuperFundPaymentViewModel, TResponse>(uri, superFundPaymentViewModel, throwOnFailure: false);

            PerformanceTestTool.EndTransaction("API: Post Super Fund Payment");

            PerformanceTestTool.LogInformation("API: Post Super Fund Payment");

            return response;
        }
    }
}

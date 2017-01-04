using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class BeneficiaryClient : SalesPortalApiClient
    {
        public async Task<BeneficiaryOptionsRequest> GetOptionsAsync(string quoteReferenceNumber, int riskId)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/risk/{riskId}/beneficiaries/options");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Beneficiary Options";
            PerformanceTestTool.StartTransaction(tran);
            
            var response = await Client.GetAsync<BeneficiaryOptionsRequest> (uri);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        public async Task<IEnumerable<BeneficiaryDetailsRequest>> GetBeneficiariesAsync(string quoteReferenceNumber, int riskId)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/risk/{riskId}/beneficiaries");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Beneficiaries";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<IEnumerable<BeneficiaryDetailsRequest>>(uri);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }

        //TODO: not working at the moment because client doesnt support GET with not content
        public async Task<TResponse> ValidateAsync<TResponse>(string quoteReferenceNumber, int riskId, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/risk/{riskId}/beneficiaries/validate");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Validate Beneficiaries";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<TResponse>(uri, false);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }
        

        public async Task<TResponse> CreateUpdateBeneficiariesAsync<TResponse>(string quoteReferenceNumber, int riskId,
            IEnumerable<BeneficiaryDetailsRequest> beneficiaryDetails, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/risk/{riskId}/beneficiaries");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Create/Update Beneficiaries";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<IEnumerable<BeneficiaryDetailsRequest>, TResponse>(uri, beneficiaryDetails, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");

            return response;
        }
        
        public async Task UpdateBeneficiaryOptionsAsync(string quoteReferenceNumber, int riskId, BeneficiaryOptionsRequest options, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/risk/{riskId}/beneficiaries/options");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Update Beneficiary Options";
            PerformanceTestTool.StartTransaction(tran);

            await Client.PostAsync(uri, options, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");
        }

        public async Task DeleteBeneficiaryAsync(string quoteReferenceNumber, int riskId, int beneficiaryId)
        {
            var uri = Client.CreateUri($"api/policy/{quoteReferenceNumber}/risk/{riskId}/beneficiaries/{beneficiaryId}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Single Beneficiary";
            PerformanceTestTool.StartTransaction(tran);

            await Client.DeleteAsync(uri);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation($"{tran} Done");
        }
    }
}
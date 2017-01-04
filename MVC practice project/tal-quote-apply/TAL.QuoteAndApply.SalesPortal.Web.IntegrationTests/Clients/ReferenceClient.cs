using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class ReferenceClient : SalesPortalApiClient
    {
        public async Task<IEnumerable<BeneficiaryRelationship>> GetBeneficiaryRelationshipsAsync()
        {
            var uri = Client.CreateUri("api/reference/beneficiaryRelationships");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get Beneficiary Relationships");

            var response = await Client.GetAsync<IEnumerable<BeneficiaryRelationship>>(uri);

            PerformanceTestTool.EndTransaction("API: Get Beneficiary Relationships");

            PerformanceTestTool.LogInformation("API: Get Beneficiary Relationships Done");

            return response;
        }
    }
}
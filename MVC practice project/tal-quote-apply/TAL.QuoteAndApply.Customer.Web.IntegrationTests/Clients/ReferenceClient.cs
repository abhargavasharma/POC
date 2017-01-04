using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients
{
    public class ReferenceClient : CustomerPortalApiClient
    {
        public async Task<IList<BeneficiaryRelationship>> GetBeneficiaryRelationships(bool throwOnFailure = true, bool pause = true)
        {
            var uri = Client.CreateUri("api/reference/beneficiaryRelationships");

            if(pause)
                PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Beneficiary Relationships";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<IList<BeneficiaryRelationship>>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class NoteClient : SalesPortalApiClient
    {
        public async Task<TResponse> EditNoteAsync<TResponse>(string quoteReference, NoteUpdateRequest noteUpdateRequest, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/policy/{0}/note/", quoteReference);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Edit Note");

            var response = await Client.PostAsync<NoteUpdateRequest, TResponse>(uri, noteUpdateRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Edit Note");

            PerformanceTestTool.LogInformation("API: Edit Note Done");

            return response;
        }

        public async Task<PolicyNotesResponse> GetNoteAsync(string quoteReference, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/policy/{0}/notes/", quoteReference);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get Note");

             var response = await Client.GetAsync<PolicyNotesResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: Get Note");

            PerformanceTestTool.LogInformation("API: Get Note Done");

            return response;
        }


    }
}

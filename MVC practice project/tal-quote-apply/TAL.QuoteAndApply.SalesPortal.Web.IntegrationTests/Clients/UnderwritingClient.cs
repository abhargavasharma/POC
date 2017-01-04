using System;
using System.Threading.Tasks;
using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.ApiClients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients
{
    public class UnderwritingClient : SalesPortalApiClient
    { 
        public async Task<UnderwritingCompleteResponse> GetUnderwritingStatusAsync(string quoteReferenceNumber, int riskId)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/underwriting/status", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get Underwriting Status");

            var response = await Client.GetAsync<UnderwritingCompleteResponse>(uri);

            PerformanceTestTool.EndTransaction("API: Get Underwriting Status");

            PerformanceTestTool.LogInformation("API: Get Underwriting Status");

            return response;
        }

        public async Task<string> GetTalusUiUrlAsync(string quoteReferenceNumber, int riskId)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/underwriting/talusUiUrl", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Get Talus Ui Url");

            var response = await Client.GetAsync<string>(uri);

            PerformanceTestTool.EndTransaction("API: Get Talus Ui Url");

            PerformanceTestTool.LogInformation("API: Get Talus Ui Url Done");

            return response;
        }

        public async Task<UnderwritingSyncResponse> QuestionAnsweredAsync(string quoteReferenceNumber, int riskId, UnderwritingQuestionAnswerRequest questionAnswerRequest)
        {
            var uri = Client.CreateUri("api/policy/{0}/risk/{1}/underwriting/answer", quoteReferenceNumber, riskId);

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Post Underwriting Question Answered");

            var response = await Client.PostAsync<UnderwritingQuestionAnswerRequest, UnderwritingSyncResponse>(uri, questionAnswerRequest, throwOnFailure: false);

            PerformanceTestTool.EndTransaction("API: Post Underwriting Question Answered");

            PerformanceTestTool.LogInformation("API: Post Underwriting Question Answered Done");

            return response;
        }
    }
}
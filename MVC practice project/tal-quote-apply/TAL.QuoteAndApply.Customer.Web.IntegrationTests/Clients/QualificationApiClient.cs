using System;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients
{
    public class QualificationApiClient : CustomerPortalApiClient
    {
        public async Task<TResponse> GetUnderwritingRisksAsync<TResponse>(bool throwOnFailure = true, bool pause = true)
        {
            var uri = Client.CreateUri("api/qualification/risks");
            
            if(pause)
                PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Underwriting Risks";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<TResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<TResponse> GetUnderwritingForRiskAsync<TResponse>(int riskId, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/qualification/risk/{riskId}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Underwriting for Risk";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<TResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }


        public async Task<TResponse> AnswerQuestionForRiskAsync<TResponse>(int riskId, UpdateQuestionRequest updateQuestionRequest, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/qualification/risk/{riskId}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Answer Question";
            PerformanceTestTool.StartTransaction(tran);
            PerformanceTestTool.StartTransaction($"{tran}: {updateQuestionRequest.QuestionId}");

            var response = await Client.PostAsync<UpdateQuestionRequest, TResponse>(uri, updateQuestionRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction($"{tran}: {updateQuestionRequest.QuestionId}");
            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<RedirectToResponse> ValidateRisk(int riskId, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/qualification/risk/{riskId}/validate");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Underwriting Risks";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<RedirectToResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }
    }
}

using System;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients
{
    public class ReviewClient : CustomerPortalApiClient
    {
        public async Task<PolicyReviewResponse> GetReviewForRiskAsync(int riskId, bool throwOnFailure = true, bool pause = true)
        {
            var uri = Client.CreateUri($"api/review/risk/{riskId}");

            if(pause)
                PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Review";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<PolicyReviewResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<PolicyReviewResponse> UpdateChoiceQuestionsAsync(int riskId, UpdateQuestionRequest updateQuestionRequest, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/review/risk/{riskId}");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));
            PerformanceTestTool.StartTransaction("API: Begin Change Loading/Exclusion Review");

            var response = await Client.PostAsync<UpdateQuestionRequest, PolicyReviewResponse>(uri, updateQuestionRequest, throwOnFailure);

            PerformanceTestTool.EndTransaction("API: End Change Loading/Exclusion Review");

            PerformanceTestTool.LogInformation("API: Get Change Loading/Exclusion complete");

            return response;
        }

        public async Task<TResponse> SetPremiumTypeAsync<TResponse>(int riskId, PremiumTypeUpdateRequest premiumTypeUpdate, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/review/risk/{riskId}/PremiumType");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Update Premium Type";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<PremiumTypeUpdateRequest, TResponse>(uri, premiumTypeUpdate, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<RedirectToResponse> ValidateReviewForRiskAsync(int riskId, bool throwOnFailure = true)
        {
            var uri = Client.CreateUri($"api/review/risk/{riskId}/validate");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Get Validate Review";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<RedirectToResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }
    }
}

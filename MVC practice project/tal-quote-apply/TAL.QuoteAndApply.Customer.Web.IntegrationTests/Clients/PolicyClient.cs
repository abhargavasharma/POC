using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients
{
    public class PolicyClient : CustomerPortalApiClient
    {
        public async Task<TResponse> InitAsync<TResponse>(bool throwOnFailure = true)
        {
            var uri = Client.CreateUri("api/policy/init");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Init basic info";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.GetAsync<TResponse>(uri, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<TResponse> CreateQuoteAsync<TResponse>(BasicInfoViewModel basicInfoViewModel, bool throwOnFailure = false, Action<string> setCookieCallback = null)
        {
            Action<HttpResponseHeaders> headerReader = null;
            if (setCookieCallback != null)
            {
                headerReader = (headers =>
                {
                    IEnumerable<string> cookieHeaders;
                    if (headers.TryGetValues("Set-Cookie", out cookieHeaders))
                    { 
                        var first = cookieHeaders.FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(first))
                            setCookieCallback(first);
                    }
                });
            }
            var uri = Client.CreateUri("api/policy/create?ignore.recaptcha=true");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: create policy";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<BasicInfoViewModel, TResponse>(uri, basicInfoViewModel, throwOnFailure, getResponseHeadersAction: headerReader);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<TResponse> CreateQuoteViaHelpMeChooseAsync<TResponse>(BasicInfoViewModel basicInfoViewModel, bool throwOnFailure = false, Action<string> setCookieCallback = null)
        {
            Action<HttpResponseHeaders> headerReader = null;
            if (setCookieCallback != null)
            {
                headerReader = (headers =>
                {
                    IEnumerable<string> cookieHeaders;
                    if (headers.TryGetValues("Set-Cookie", out cookieHeaders))
                    {
                        var first = cookieHeaders.FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(first))
                            setCookieCallback(first);
                    }
                });
            }
            var uri = Client.CreateUri("api/policy/create-for-help-me-choose");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: create policy create-for-help-me-choose";
            PerformanceTestTool.StartTransaction(tran);

            var response = await Client.PostAsync<BasicInfoViewModel, TResponse>(uri, basicInfoViewModel, throwOnFailure, getResponseHeadersAction: headerReader);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return response;
        }

        public async Task<bool> ValidateGeneralInformationAsync(GeneralInformationViewModel generalInformation, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/policy/validate/generalInformation");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Policy Validate generalInformation";
            PerformanceTestTool.StartTransaction(tran);

            var result = await Client.PostAsync(uri, generalInformation, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return result;
        }

        public async Task<TResponse> ValidateGeneralInformationAsync<TResponse>(GeneralInformationViewModel generalInformation, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/policy/validate/generalInformation");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Policy Validate generalInformation";
            PerformanceTestTool.StartTransaction(tran);

            var result = await Client.PostAsync<GeneralInformationViewModel, TResponse>(uri, generalInformation, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return result;
        }

        public async Task<TResponse> ValidateAgeAsync<TResponse>(ValidateAgeViewModel validateAgeViewModel, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/policy/validate/age");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Policy Validate age";
            PerformanceTestTool.StartTransaction(tran);

            var result = await Client.PostAsync<ValidateAgeViewModel, TResponse>(uri, validateAgeViewModel, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return result;
        }

        public async Task<bool> ValidateIncomeAsync(IncomeViewModel income, bool throwOnFailure = false)
        {
            var uri = Client.CreateUri($"api/policy/validate/income");

            PerformanceTestTool.Pause(TimeSpan.FromTicks(0));

            const string tran = "API: Policy Validate Age";
            PerformanceTestTool.StartTransaction(tran);

            var result = await Client.PostAsync(uri, income, throwOnFailure);

            PerformanceTestTool.EndTransaction(tran);

            PerformanceTestTool.LogInformation(tran);

            return result;
        }

    }
}

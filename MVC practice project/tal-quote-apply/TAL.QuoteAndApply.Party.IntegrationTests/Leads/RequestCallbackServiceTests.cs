using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Party.Leads.CallbackService;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Party.IntegrationTests.Leads
{
    [TestFixture]
    public class RequestCallbackServiceTests
    {
        private readonly RequestCallbackService _service;

        public RequestCallbackServiceTests()
        {
            var httpClientService = new HttpClientService(new HttpResponseMessageSerializer(), new MimeTypeProvider(),
                new HttpRequestMessageSerializer());
            
            _service =  new RequestCallbackService(new LeadConfigurationProvider(), httpClientService, new MockLoggingService(), new UrlUtilities());
        }

        [Test]
        public async void RequestCallback()
        {
            var response = await _service.RequestCallback("1", "0400000000");
            Assert.IsNotNull(response);
            Assert.AreEqual(response.Status, "ok");
            Assert.AreEqual(response.ErrorCode, 0);
        }


    }
}

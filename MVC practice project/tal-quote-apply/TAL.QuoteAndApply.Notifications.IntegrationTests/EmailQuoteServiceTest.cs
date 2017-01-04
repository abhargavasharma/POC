using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Infrastructure.Mail;
using TAL.QuoteAndApply.Infrastructure.Resource;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Notifications.Configuration;
using TAL.QuoteAndApply.Notifications.Models;
using TAL.QuoteAndApply.Notifications.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Notifications.IntegrationTests
{
    [TestFixture]
    public class EmailQuoteServiceTest
    {
        [Test, Ignore]
        public void Test()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var context = mockRepository.Create<HttpContextBase>();
            var request = mockRepository.Create<HttpRequestBase>();
            var response = mockRepository.Create<HttpResponseBase>();
            var session = mockRepository.Create<HttpSessionStateBase>();
            var server = mockRepository.Create<HttpServerUtilityBase>();
            var user = mockRepository.Create<IPrincipal>();
            var identity = mockRepository.Create<IIdentity>();
            var items = new Hashtable();

            request.Setup(call => call.Url)
                .Returns(new Uri("http://test-talcustomer.delivery.lan/abc/def?abc=123"));

            context.Setup(h => h.Request).Returns(request.Object);
            context.Setup(h => h.Response).Returns(response.Object);

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            context.Setup(ctx => ctx.Items).Returns(items);

            identity.Setup(x => x.Name).Returns("TAL.QuoteAndApply.Customer.Web.IntegrationTests");
            user.Setup(h => h.Identity).Returns(identity.Object);
            context.Setup(h => h.User).Returns(user.Object);

            var mockHttpContextProvider = mockRepository.Create<IHttpContextProvider>();
            mockHttpContextProvider.Setup(call => call.GetCurrentContext()).Returns(context.Object);
                
            var emailQuoteService = new EmailQuoteService(new SmtpService(new MockLoggingService(), new SmtpClientFactory(), new MailMessageBuilder()), new ResourceFileReader(), 
                new EmailQuoteViewModelProvider(new EmailConfigurationProvider(), new CurrentUrlProvider(mockHttpContextProvider.Object)), new EmailConfigurationProvider(), new EmailTemplateConstantsProvider());
            emailQuoteService.SendQuote("M1234567", "Chris.Moretton@tal.com.au", "ChristobellSemiautoWeapon", "TAL");
        }
    }
}

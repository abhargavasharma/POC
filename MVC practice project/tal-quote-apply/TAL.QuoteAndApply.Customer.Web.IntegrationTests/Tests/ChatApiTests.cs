using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Customer.Web.Models;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.DataModel.Interactions;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests
{
    [TestFixture]
    public class ChatApiTests : BaseTestClass<ChatClient>
    {
        public ChatApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [Test]
        public async Task TestRequestCallbackWithRiskId_Async()
        {
            //Arrange
            var policyResults = CreateAndLoginAsDefaultPolicy();

            //Act
            var request = new ChatRequestCallbackRequest
            {
                EmailAddress = "user@company.com",
                FirstName = "first",
                LastName = "Test",
                PhoneNumber = "0400000000",
                RiskId = policyResults.Risk.Id
            };
            var response = await Client.RequestCallbackAsync<ChatRequestCallbackRequest, bool>(request);

            //Assert
            Assert.IsTrue(response);
            Assert.That(QuoteSessionContext.QuoteSession.SessionData.CallBackRequested, Is.EqualTo(true));
        }

        [Test]
        public async Task TestRequestCallbackWithoutRiskId_Async()
        {
            //Arrange
            CreateAndLoginAsDefaultPolicy();

            //Act
            var request = new ChatRequestCallbackRequest
            {
                FirstName = "Butlerrrrrrrrrrrrrrrrrrrrr",
                LastName = "Testtttttttttttttttttttttt",
                PhoneNumber = "0400000000",
                EmailAddress = "step@gmail.commmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm"
            };
            var response =
                await Client.RequestCallbackAsync<ChatRequestCallbackRequest, Dictionary<string, IEnumerable<string>>>(request,
                    false);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("request.riskId"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("request.riskId")).Value.First(), Is.EqualTo("Risk Id is required"));
            Assert.That(response.First(x => x.Key.Equals("request.firstName")).Value.First(),
                Is.EqualTo("First name cannot be longer than 22 characters."));
            Assert.That(response.First(x => x.Key.Equals("request.lastName")).Value.First(),
                Is.EqualTo("Last name cannot be longer than 22 characters."));
            Assert.That(response.First(x => x.Key.Equals("request.emailAddress")).Value.First(),
                Is.EqualTo("Email address cannot be longer than 80 characters."));
            Assert.That(QuoteSessionContext.QuoteSession.SessionData.CallBackRequested, Is.EqualTo(false));
        }


        [Test]
        public async Task TectGetAvailabilityWithValidPolicy_Async()
        {
            var policy = CreateAndLoginAsDefaultPolicy();

            var availablity = await Client.GetAvailabilityAsync(false);
            
            Assert.That(availablity.WebChatUrl, Is.Not.Empty);
            Assert.That(availablity.WebChatUrl, Is.StringContaining(HttpUtility.UrlEncode(policy.Policy.QuoteReference)));
            Assert.That(availablity.WebChatUrl, Is.StringContaining(HttpUtility.UrlEncode(policy.Party.EmailAddress)));
            Assert.That(availablity.WebChatUrl, Is.StringContaining(HttpUtility.UrlEncode(policy.Party.FirstName)));
            Assert.That(availablity.WebChatUrl, Is.StringContaining(HttpUtility.UrlEncode(policy.Party.Surname)));
            Assert.That(availablity.WebChatUrl, Is.StringContaining(HttpUtility.UrlEncode(policy.Party.MobileNumber)));
        }

        [Test]
        public async Task TectGetAvailabilityWithoutValidPolicy_Async()
        {
            var availablity = await Client.GetAvailabilityAsync(false);

            Assert.That(availablity.WebChatUrl, Is.Not.Empty);
            Assert.That(availablity.WebChatAvailableFrom, Is.Not.Empty);
            Assert.That(availablity.WebChatAvailableTo, Is.Not.Empty);
        }

        [Test]
        public async Task RequestWebchatAsync_WhenWebchatRequested_InteractionMustBeCreated()
        {
            //Arrange
            var policy = CreateAndLoginAsDefaultPolicy();

            //Act
            var response = await Client.RequestWebchatAsync(false);

            //Assert
            Assert.That(response.WebChatUrl, Is.Not.Empty);
            Assert.That(response.WebChatUrl, Is.StringContaining(HttpUtility.UrlEncode(policy.Policy.QuoteReference)));
            Assert.That(response.WebChatUrl, Is.StringContaining(HttpUtility.UrlEncode(policy.Party.EmailAddress)));
            Assert.That(response.WebChatUrl, Is.StringContaining(HttpUtility.UrlEncode(policy.Party.FirstName)));
            Assert.That(response.WebChatUrl, Is.StringContaining(HttpUtility.UrlEncode(policy.Party.Surname)));
            Assert.That(response.WebChatUrl, Is.StringContaining(HttpUtility.UrlEncode(policy.Party.MobileNumber)));
            Assert.That(QuoteHasInteraction(policy.Policy.QuoteReference, InteractionType.Customer_Webchat_Requested), Is.True);
        }
    }
}

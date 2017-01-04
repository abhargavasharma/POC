using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api.Results;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    public class PaymentApiTests : BaseTestClass<PaymentClient>
    {
        public PaymentApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {

        }

        [Test]
        public async Task Get_GetAvailablePaymentOptionss_ReturnsResponseWithNoErrors_Async()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var response = await Client.GetAvailablePaymentOptionsAsync(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id);

            Assert.That(!response.IsComplete);
            Assert.That(response.IsCreditCardAvailable);
            Assert.That(response.IsDirectDebitAvailable);
            Assert.That(response.IsSuperFundAvailable);
        }


        [Test]
        public async Task Update_PaymentWithCompleteDirectDebitFields_ReturnsResponseWithNoErrors_Async()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var paymentUpdate = new DirectDebitPaymentViewModel
            {
                AccountName = "Jimmy Account",
                AccountNumber = "123456789",
                BsbNumber = "123456",
                IsValidForInforce = false
            };

            var response = await Client.PayViaDirectDebitAsync<PaymentOptionsParam>(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id, paymentUpdate, false);

            Assert.That(response.DirectDebitPayment.BSBNumber == "123456");
            Assert.That(response.DirectDebitPayment.AccountNumber == "123456789");
            Assert.That(response.DirectDebitPayment.AccountName == "Jimmy Account");
            Assert.That(!response.DirectDebitPayment.IsComplete);
            Assert.That(response.DirectDebitPayment.IsValidForInforce);
        }

        [Test]
        public async Task Update_PaymentWithInCompleteDirectDebitFields_ReturnsResponseWithengthErrors_Async()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var paymentUpdate = new DirectDebitPaymentViewModel
            {
                AccountName = "22222222222222222222222222222222222222222222222222222222222",
                AccountNumber = "2333333333333",
                BsbNumber = "12345",
                IsValidForInforce = false
            };

            var response = await Client.PayViaDirectDebitAsync<Dictionary<string, IEnumerable<string>>>(
                    createPolicyResult.Policy.QuoteReference, createPolicyResult.Risk.Id, paymentUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("directDebitPayment.accountName"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("directDebitPayment.accountName")).Value.First(),
                Is.EqualTo("Account name cannot be more than 32 characters"));
            Assert.That(response.ContainsKey("directDebitPayment.bsbNumber"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("directDebitPayment.bsbNumber")).Value.First(),
                Is.EqualTo("BSB number must be exactly 6 Digits"));
            Assert.That(response.ContainsKey("directDebitPayment.accountNumber"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("directDebitPayment.accountNumber")).Value.First(),
                Is.EqualTo("Account number cannot be more than 9 Digits"));
        }

        [Test]
        public async Task Update_PaymentWithInCompleteDirectDebitFields_ReturnsResponseWithNonNumberErrors_Async()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var paymentUpdate = new DirectDebitPaymentViewModel
            {
                AccountName = "Bob Account",
                AccountNumber = "hhhhhhhh",
                BsbNumber = "12345h",
                IsValidForInforce = false
            };

            var response = await Client.PayViaDirectDebitAsync<Dictionary<string, IEnumerable<string>>>(
                    createPolicyResult.Policy.QuoteReference, createPolicyResult.Risk.Id, paymentUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("directDebitPayment.bsbNumber"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("directDebitPayment.bsbNumber")).Value.First(),
                Is.EqualTo("BSB number must not contain any non numeric values"));
            Assert.That(response.ContainsKey("directDebitPayment.accountNumber"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("directDebitPayment.accountNumber")).Value.First(),
                Is.EqualTo("Account number must not contain any non numeric values"));
        }

        [Test]
        public async Task Update_CreditCard_TokenIsNull_ReturnsResponseWithRequiredFieldErrors_Async()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var paymentUpdate = new CreditCardPaymentViewModel
            {
                Token = null
            };

            var response = await Client.PayViaCreditCardAsync<Dictionary<string, IEnumerable<string>>>(
                    createPolicyResult.Policy.QuoteReference, createPolicyResult.Risk.Id, paymentUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("creditCardPayment.cardNumber"), Is.True);
            Assert.That(response.ContainsKey("creditCardPayment.expiryMonth"), Is.True);
            Assert.That(response.ContainsKey("creditCardPayment.expiryYear"), Is.True);
            Assert.That(response.ContainsKey("creditCardPayment.nameOnCard"), Is.True);
            Assert.That(response.ContainsKey("creditCardPayment.cardType"), Is.True);
        }

        [Test]
        public async Task Update_CreditCard_TokenIsEmpty_ReturnsResponseWithRequiredFieldErrors_Async()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var paymentUpdate = new CreditCardPaymentViewModel
            {
                Token = string.Empty
            };

            var response = await Client.PayViaCreditCardAsync<Dictionary<string, IEnumerable<string>>>(
                    createPolicyResult.Policy.QuoteReference, createPolicyResult.Risk.Id, paymentUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("creditCardPayment.cardNumber"), Is.True);
            Assert.That(response.ContainsKey("creditCardPayment.cardType"), Is.True);
            Assert.That(response.ContainsKey("creditCardPayment.nameOnCard"), Is.True);
        }
    }
}
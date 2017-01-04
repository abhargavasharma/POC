using System;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Policy.Rules.Plan;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Helpers;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Services
{
    public class PaymentOptionsAvailabilityProviderTests
    {

        [SetUp]
        public void Setup()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();
        }

        [Test]
        public void GetAvailablePaymentTypes_LifeOnlyPlanSelected_AllPaymentsAvailable()
        {
            var service = new PaymentOptionsAvailabilityProvider(
                new IPaymentOptionsAvailabilityRule[]
                {
                    new CreditCardPaymentAvailabilityRule(),
                    new DirectDebitPaymentAvailabilityRule(),
                    new SuperAnnuationPaymentAvailabilityRule()
                });

            var createPolicyResult = CreatePolicyResult();

            var paymentTypes = service.GetAvailablePaymentTypes(createPolicyResult.Plans);
            Assert.That(paymentTypes, Contains.Item(PaymentType.CreditCard));
            Assert.That(paymentTypes, Contains.Item(PaymentType.DirectDebit));
            Assert.That(paymentTypes, Contains.Item(PaymentType.SuperAnnuation));

        }

        [Test]
        public void GetAvailablePaymentTypes_LifeANdIpPlanSelected_AllPaymentsAvailable()
        {
            var service = new PaymentOptionsAvailabilityProvider(
                new IPaymentOptionsAvailabilityRule[]
                {
                    new CreditCardPaymentAvailabilityRule(),
                    new DirectDebitPaymentAvailabilityRule(),
                    new SuperAnnuationPaymentAvailabilityRule()
                });

            var createPolicyResult = CreatePolicyResult();
            createPolicyResult.Plans.First(p => p.Code.Equals("IP", StringComparison.OrdinalIgnoreCase)).Selected = true;

            var paymentTypes = service.GetAvailablePaymentTypes(createPolicyResult.Plans);
            Assert.That(paymentTypes, Contains.Item(PaymentType.CreditCard));
            Assert.That(paymentTypes, Contains.Item(PaymentType.DirectDebit));
            Assert.That(paymentTypes, Contains.Item(PaymentType.SuperAnnuation));

        }

        [Test]
        public void GetAvailablePaymentTypes_LifeAndCIPlanSelected_SuperPaymentsNotAvailable()
        {
            var service = new PaymentOptionsAvailabilityProvider(
                new IPaymentOptionsAvailabilityRule[]
                {
                    new CreditCardPaymentAvailabilityRule(),
                    new DirectDebitPaymentAvailabilityRule(),
                    new SuperAnnuationPaymentAvailabilityRule()
                });

            var createPolicyResult = CreatePolicyResult();
            createPolicyResult.Plans.First(p => p.Code.Equals("TRS", StringComparison.OrdinalIgnoreCase)).Selected = true;

            var paymentTypes = service.GetAvailablePaymentTypes(createPolicyResult.Plans);
            Assert.That(paymentTypes, Contains.Item(PaymentType.CreditCard));
            Assert.That(paymentTypes, Contains.Item(PaymentType.DirectDebit));
            Assert.That(paymentTypes, !Contains.Item(PaymentType.SuperAnnuation));

        }

        [Test]
        public void GetAvailablePaymentTypes_LifeAndTPDPlanSelected_SuperPaymentsNotAvailable()
        {
            var service = new PaymentOptionsAvailabilityProvider(
                new IPaymentOptionsAvailabilityRule[]
                {
                    new CreditCardPaymentAvailabilityRule(),
                    new DirectDebitPaymentAvailabilityRule(),
                    new SuperAnnuationPaymentAvailabilityRule()
                });

            var createPolicyResult = CreatePolicyResult();
            createPolicyResult.Plans.First(p => p.Code.Equals("TPS", StringComparison.OrdinalIgnoreCase)).Selected = true;

            var paymentTypes = service.GetAvailablePaymentTypes(createPolicyResult.Plans);
            Assert.That(paymentTypes, Contains.Item(PaymentType.CreditCard));
            Assert.That(paymentTypes, Contains.Item(PaymentType.DirectDebit));
            Assert.That(paymentTypes, !Contains.Item(PaymentType.SuperAnnuation));

        }

        private static CreatePolicyResult CreatePolicyResult()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);
            return createPolicyResult;
        }
    }
}

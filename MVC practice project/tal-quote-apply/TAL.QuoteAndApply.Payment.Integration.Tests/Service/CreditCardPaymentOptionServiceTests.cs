using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TalDirect.TokenisationClient;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Configuration;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Service;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Payment.Integration.Tests.Service
{
    [TestFixture]
    public class CreditCardPaymentOptionServiceTests
    {
        private PaymentConfigurationProvider _config;
        private CreditCardPaymentRepository _creditCardRepo;
        private PolicyPaymentRepository _policyPaymentRepo;

        public CreditCardPaymentOptionServiceTests()
        {
            _config = new PaymentConfigurationProvider();
            _creditCardRepo = new CreditCardPaymentRepository(_config, new MockCurrentUserProvider(),
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
            _policyPaymentRepo = new PolicyPaymentRepository(_config, new MockCurrentUserProvider(),
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
        }

        [SetUp]
        public void RegisterMappers()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();
        }
        
        
        [Test]
        public void AssignCreditCardPayment_NoExistingCreditCard_CreatesNewCreditCardAndPolicyPaymentRecord()
        {
            var tokenisationFactory = new CreditCardTokenisationServiceFactory(_config);

            var policy = PolicyHelper.CreatePolicy();

            var service = new CreditCardPaymentOptionService(_policyPaymentRepo, tokenisationFactory, _creditCardRepo, new MockLoggingService());
            var creditCardDto = service.AssignCreditCardPayment(policy.Id, "NameOnCard", "4111222233334444", "05", "50", CreditCardType.MasterCard);

            Assert.That(creditCardDto.NameOnCard, Is.EqualTo("NameOnCard"));
            Assert.That(creditCardDto.CardNumber, Is.Not.EqualTo("4111222233334444"));
            Assert.That(creditCardDto.CardNumber, Is.EqualTo("411122*******444"));
            Assert.That(creditCardDto.Token, Is.Not.Null);

            var paymentDto = _policyPaymentRepo.GetForPolicy(policy.Id);

            Assert.That(paymentDto.FirstOrDefault(), Is.Not.Null);
            Assert.That(paymentDto.FirstOrDefault().PaymentType, Is.EqualTo(PaymentType.CreditCard));

            Assert.That(creditCardDto.PolicyPaymentId, Is.EqualTo(paymentDto.FirstOrDefault().Id));
        }

        [Test]
        public void AssignCreditCardPayment_ExistingCreditCard_CreatesNewCreditCardAndUsesExistingPolicyPaymentRecord()
        {
            var tokenisationFactory = new CreditCardTokenisationServiceFactory(_config);

            var policy = PolicyHelper.CreatePolicy();

            var service = new CreditCardPaymentOptionService(_policyPaymentRepo, tokenisationFactory, _creditCardRepo, new MockLoggingService());
            var creditCardDto1 = service.AssignCreditCardPayment(policy.Id, "NameOnCard", "5444444444444444", "01", "50", CreditCardType.MasterCard);
            var paymentDtoFirst = _policyPaymentRepo.GetForPolicy(policy.Id);

            var creditCardDto2 = service.AssignCreditCardPayment(policy.Id, "NewNameOnCard", "5111111111111111", "01", "50", CreditCardType.MasterCard);
            var paymentDtoSecond = _policyPaymentRepo.GetForPolicy(policy.Id);

            Assert.That(creditCardDto1.Id, Is.Not.EqualTo(creditCardDto2.Id));
            Assert.That(paymentDtoFirst.FirstOrDefault().Id, Is.EqualTo(paymentDtoSecond.FirstOrDefault().Id));

            var shouldBeDeletedDto = _creditCardRepo.Get(creditCardDto1.Id);
            Assert.That(shouldBeDeletedDto, Is.Null);
        }
    }
}

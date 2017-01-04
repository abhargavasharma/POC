using System;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Data
{
    [TestFixture]
    public class ReferralDtoRepositoryTests
    {
        [Test]
        public void GetForPolicy_NoReferral_ReturnsNull()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var insertedPolicy = PolicyCreator.CreatePolicy();

            var config = new PolicyConfigurationProvider();
            var currentUserProvider = new MockCurrentUserProvider();

            var repo = new ReferralDtoRepository(config,
                currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var result = repo.GetForPolicy(insertedPolicy.Id);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetInprogressReferralForPolicy_NoInprogress_NullReturned()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var insertedPolicy = PolicyCreator.CreatePolicy();

            var config = new PolicyConfigurationProvider();
            var currentUserProvider = new MockCurrentUserProvider();

            var repo = new ReferralDtoRepository(config,
                currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var referral = new ReferralDto { PolicyId = insertedPolicy.Id, IsCompleted = true};

            repo.CreateReferral(referral);

            var result = repo.GetInprogressReferralForPolicy(insertedPolicy.Id);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetInprogressReferralForPolicy_InprogressReferral_Returned()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var insertedPolicy = PolicyCreator.CreatePolicy();

            var config = new PolicyConfigurationProvider();
            var currentUserProvider = new MockCurrentUserProvider();

            var repo = new ReferralDtoRepository(config,
                currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var referral = new ReferralDto { PolicyId = insertedPolicy.Id, IsCompleted = false };

            repo.CreateReferral(referral);

            var result = repo.GetInprogressReferralForPolicy(insertedPolicy.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void GetInprogressReferralForPolicy_MultipleInProgressReferralsForPolicy_ExceptionThrown()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var insertedPolicy = PolicyCreator.CreatePolicy();

            var config = new PolicyConfigurationProvider();
            var currentUserProvider = new MockCurrentUserProvider();

            var repo = new ReferralDtoRepository(config,
                currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var referral1 = new ReferralDto { PolicyId = insertedPolicy.Id, IsCompleted = false };
            var referral2 = new ReferralDto { PolicyId = insertedPolicy.Id, IsCompleted = false };

            repo.CreateReferral(referral1);
            repo.CreateReferral(referral2);

            repo.GetInprogressReferralForPolicy(insertedPolicy.Id);
        }

        [Test]
        public void CreateReferral_GetForPolicy_ReturnsReferral()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var insertedPolicy = PolicyCreator.CreatePolicy();

            var config = new PolicyConfigurationProvider();
            var currentUserProvider = new MockCurrentUserProvider();

            var repo = new ReferralDtoRepository(config,
                currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var referral = new ReferralDto {PolicyId = insertedPolicy.Id};

            repo.CreateReferral(referral);

            var result = repo.GetForPolicy(insertedPolicy.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void UpdateReferral()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var insertedPolicy = PolicyCreator.CreatePolicy();

            var config = new PolicyConfigurationProvider();
            var currentUserProvider = new MockCurrentUserProvider();

            var repo = new ReferralDtoRepository(config,
                currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var referral = new ReferralDto { PolicyId = insertedPolicy.Id };

            repo.CreateReferral(referral);

            var getReferral = repo.GetForPolicy(insertedPolicy.Id);
            Assert.That(getReferral, Is.Not.Null);
            Assert.That(getReferral.AssignedTo, Is.Null);
            Assert.That(getReferral.AssignedToTS, Is.Null);
            Assert.That(getReferral.CompletedTS, Is.Null);
            Assert.That(getReferral.IsCompleted, Is.False);

            getReferral.AssignedTo = "Test.User";
            getReferral.AssignedToTS = DateTime.Today;

            repo.UpdateReferral(getReferral);

            var getReferral2 = repo.GetForPolicy(insertedPolicy.Id);

            Assert.That(getReferral2, Is.Not.Null);
            Assert.That(getReferral2.AssignedTo, Is.EqualTo(getReferral.AssignedTo));
            Assert.That(getReferral2.AssignedToTS, Is.EqualTo(getReferral.AssignedToTS));
            Assert.That(getReferral2.CompletedTS, Is.Null);
            Assert.That(getReferral2.IsCompleted, Is.False);

            getReferral2.IsCompleted = true;
            getReferral2.CompletedTS = DateTime.Today;

            repo.UpdateReferral(getReferral2);

            var getReferral3 = repo.GetForPolicy(insertedPolicy.Id);

            Assert.That(getReferral3, Is.Not.Null);
            Assert.That(getReferral3.AssignedTo, Is.EqualTo(getReferral.AssignedTo));
            Assert.That(getReferral3.AssignedToTS, Is.EqualTo(getReferral.AssignedToTS));
            Assert.That(getReferral3.CompletedTS, Is.EqualTo(getReferral2.CompletedTS));
            Assert.That(getReferral3.IsCompleted, Is.EqualTo(getReferral2.IsCompleted));
        }

        [Test]
        public void GetCompletedReferralsForPolicy_NoCompleteReferrals_ReturnsEmptyList()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var insertedPolicy = PolicyCreator.CreatePolicy();

            var config = new PolicyConfigurationProvider();
            var currentUserProvider = new MockCurrentUserProvider();

            var repo = new ReferralDtoRepository(config,
                currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var referral = new ReferralDto { PolicyId = insertedPolicy.Id };

            repo.CreateReferral(referral);

            var results = repo.GetCompletedReferralsForPolicy(insertedPolicy.Id);
            
            CollectionAssert.IsEmpty(results);
        }

        [Test]
        public void GetCompletedReferralsForPolicy_MultipleCompletedReferrals_ReturnsList()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var insertedPolicy = PolicyCreator.CreatePolicy();

            var config = new PolicyConfigurationProvider();
            var currentUserProvider = new MockCurrentUserProvider();

            var repo = new ReferralDtoRepository(config,
                currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var referral1 = new ReferralDto { PolicyId = insertedPolicy.Id, CompletedTS = DateTime.Now, IsCompleted = true, };
            var referral2 = new ReferralDto { PolicyId = insertedPolicy.Id, CompletedTS = DateTime.Now, IsCompleted = true, };

            repo.CreateReferral(referral1);
            repo.CreateReferral(referral2);

            var results = repo.GetCompletedReferralsForPolicy(insertedPolicy.Id);

            Assert.That(results.Count(), Is.EqualTo(2));
        }
    }
}

using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Data;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Data
{
    [TestFixture]
    public class PolicyOwnerDtoRepositoryTests
    {
        [Test]
        public void Insert_Get_Update()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var partyOne = GetAParty();
            var partyTwo = GetAParty();

            Assert.That(partyOne.Id, Is.GreaterThan(0));
            Assert.That(partyTwo.Id, Is.GreaterThan(0));

            var insertedPolicy = PolicyCreator.CreatePolicy();

            var config = new PolicyConfigurationProvider();
            var currentUserProvider = new MockCurrentUserProvider();
            
            var repo = new PolicyOwnerDtoRepository(config, 
                currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var initialPolicyOwnerParty = new PolicyOwnerDto
            {
                PolicyId = insertedPolicy.Id,
                PartyId = partyOne.Id
            };

            var insertedPolicyOwner = repo.Insert(initialPolicyOwnerParty);
            Assert.That(insertedPolicyOwner.Id, Is.GreaterThan(0));

            var getPolicyOwner = repo.GetPolicyOwnerForPolicyId(insertedPolicy.Id);
            Assert.That(getPolicyOwner.Id, Is.GreaterThan(0));
            Assert.That(getPolicyOwner.PartyId, Is.EqualTo(partyOne.Id));
            Assert.That(getPolicyOwner.PolicyId, Is.EqualTo(insertedPolicy.Id));

            getPolicyOwner.PartyId = partyTwo.Id;
            repo.Update(getPolicyOwner);

            var getPolicyOwner2 = repo.GetPolicyOwnerForPolicyId(insertedPolicy.Id);
            Assert.That(getPolicyOwner2.Id, Is.GreaterThan(0));
            Assert.That(getPolicyOwner2.PartyId, Is.EqualTo(partyTwo.Id));
            Assert.That(getPolicyOwner2.PolicyId, Is.EqualTo(insertedPolicy.Id));
        }

        private static PartyDto GetAParty()
        {
            var partyConfig = new PartyConfigurationProvider();

            var partyRepo = new PartyDtoRepository(partyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(),
                new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var party = new PartyDto()
            {
                DateOfBirth = new DateTime(1980, 4, 22),
                Gender = Gender.Male,
                Title = Title.Mr,
                FirstName = Guid.NewGuid().ToString(),
                Surname = Guid.NewGuid().ToString(),
                Address = "132 ABC St",
                Suburb = "Melbourne",
                State = State.VIC,
                Country = Country.Australia,
                Postcode = "1234",
                MobileNumber = "0411111111",
                HomeNumber = "0711111111",
                EmailAddress = "chris@chris.com"
            };

            var insertedParty = partyRepo.InsertParty(party);
            return insertedParty;
        }
    }
}

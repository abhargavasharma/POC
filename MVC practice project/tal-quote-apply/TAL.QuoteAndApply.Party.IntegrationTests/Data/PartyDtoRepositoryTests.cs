using System;
using System.Linq;
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
using TAL.QuoteAndApply.Tests.Shared.Mocks;


namespace TAL.QuoteAndApply.Party.IntegrationTests.Data
{
    [TestFixture]
    public class PartyDtoRepositoryTests
    {
        [Test]
        public void Insert_Get_Update()
        {
            var config = new PartyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PartyDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(),
                new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var party = new PartyDto()
            {
                DateOfBirth = new DateTime(1980, 4, 22),
                Gender = Gender.Male,
                Title = Title.Mr,
                FirstName = "Chris",
                Surname = "Test",
                Address = "132 ABC St",
                Suburb = "Melbourne",
                State = State.VIC,
                Country = Country.Australia,
                Postcode = "1234",
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "chris@chris.com"
            };

            var insertedParty = repo.InsertParty(party);

            Assert.That(insertedParty.Id, Is.GreaterThan(0));

            var getClient = repo.GetParty(insertedParty.Id);

            Assert.That(getClient, Is.Not.Null);

            Assert.That(getClient.Gender, Is.EqualTo(party.Gender));
            Assert.That(getClient.DateOfBirth, Is.EqualTo(party.DateOfBirth));

            Assert.That(getClient.Title, Is.EqualTo(party.Title));
            Assert.That(getClient.FirstName, Is.EqualTo(party.FirstName));
            Assert.That(getClient.Surname, Is.EqualTo(party.Surname));


            Assert.That(getClient.Address, Is.EqualTo(party.Address));
            Assert.That(getClient.Suburb, Is.EqualTo(party.Suburb));
            Assert.That(getClient.State, Is.EqualTo(party.State));
            Assert.That(getClient.Country, Is.EqualTo(party.Country));
            Assert.That(getClient.Postcode, Is.EqualTo(party.Postcode));

            Assert.That(getClient.MobileNumber, Is.EqualTo(party.MobileNumber));
            Assert.That(getClient.HomeNumber, Is.EqualTo(party.HomeNumber));
            Assert.That(getClient.EmailAddress, Is.EqualTo(party.EmailAddress));

            getClient.Title = Title.Mrs;

            bool updateResult = true;
            try
            {
                repo.UpdateParty(getClient);
            }
            catch (Exception ex)
            {
                updateResult = false;
            }

            Assert.That(updateResult, Is.True);


            var dbItem2 = repo.GetParty(insertedParty.Id);

            Assert.That(dbItem2.Title, Is.EqualTo(Title.Mrs));
        }

        [Test]
        public void DeleteTest()
        {
            var config = new PartyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PartyDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(),
                new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var insertedParty = repo.InsertParty(new PartyDto()
            {
                DateOfBirth = new DateTime(1980, 4, 22),
                Gender = Gender.Male
            });
            Assert.That(insertedParty.Id, Is.GreaterThan(0));

            repo.DeleteParty(insertedParty.Id);
            
            var deletedParty = repo.GetParty(insertedParty.Id);

            Assert.That(deletedParty, Is.Null);
        }

        [Test]
        public void GetByLeadIdTest()
        {
            var config = new PartyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PartyDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(),
                new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));
            
            var leadId = new Random().Next(int.MaxValue);

            //insert 10 parties
            var parties = Enumerable.Range(0, 10).Select(i => repo.InsertParty(new PartyDto()
            {
                DateOfBirth = new DateTime(1980, 4, 22),
                FirstName = "Person" + i,
                Gender = Gender.Male,
                LeadId = leadId
            })).ToList();

            var deletedParty = repo.InsertParty(new PartyDto()
            {
                DateOfBirth = new DateTime(1980, 4, 22),
                FirstName = "Dont Shoot",
                Gender = Gender.Male,
                LeadId = leadId
            });
            repo.DeleteParty(deletedParty.Id);

            var insertedParties = repo.GetPartiesByLeadId(leadId);

            Assert.That(insertedParties.Count, Is.EqualTo(10));
            Assert.That(insertedParties.All(p => p.FirstName != deletedParty.FirstName));
        }
    }
}

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
using TAL.QuoteAndApply.Party.IntegrationTests.Data.Encryption;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Tests.Shared.Mocks;


namespace TAL.QuoteAndApply.Party.IntegrationTests.Data
{
    [TestFixture]
    public class EncryptedPartyDtoRepositoryTests
    {
        [Test]
        public void Insert_Get_Update_Delete()
        {
            var config = new PartyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var encryptionRepo = new EncryptedPartyDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(),
                new DbItemEncryptionService(new SimpleEncryptionService()));
            var regularRepo = new PartyDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(),
                new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var party = new EncryptedPartyDto
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

            var insertedParty = encryptionRepo.InsertParty(party);

            Assert.That(insertedParty.Id, Is.GreaterThan(0));

            var getClient = encryptionRepo.GetParty(insertedParty.Id);

            Assert.That(getClient, Is.Not.Null);

            Assert.That(getClient.Gender, Is.EqualTo(insertedParty.Gender));
            Assert.That(getClient.DateOfBirth, Is.EqualTo(insertedParty.DateOfBirth));

            Assert.That(getClient.Title, Is.EqualTo(insertedParty.Title));
            Assert.That(getClient.FirstName, Is.EqualTo(insertedParty.FirstName));
            Assert.That(getClient.Surname, Is.EqualTo(insertedParty.Surname));


            Assert.That(getClient.Address, Is.EqualTo(insertedParty.Address));
            Assert.That(getClient.Suburb, Is.EqualTo(insertedParty.Suburb));
            Assert.That(getClient.State, Is.EqualTo(insertedParty.State));
            Assert.That(getClient.Country, Is.EqualTo(insertedParty.Country));
            Assert.That(getClient.Postcode, Is.EqualTo(insertedParty.Postcode));

            Assert.That(getClient.MobileNumber, Is.EqualTo(insertedParty.MobileNumber));
            Assert.That(getClient.HomeNumber, Is.EqualTo(insertedParty.HomeNumber));
            Assert.That(getClient.EmailAddress, Is.EqualTo(insertedParty.EmailAddress));

            getClient.Title = Title.Mrs;

            var rawPartyDto = regularRepo.GetParty(insertedParty.Id);
            Assert.That(rawPartyDto.EmailAddress, Is.Not.EqualTo(getClient.EmailAddress));

            var updateResult = true;
            try
            {
                encryptionRepo.UpdateParty(getClient);
            }
            catch (Exception ex)
            {
                updateResult = false;
            }
            

            Assert.That(updateResult, Is.True);

            var getClient2 = encryptionRepo.GetParty(insertedParty.Id);

            Assert.That(getClient2.Title, Is.EqualTo(Title.Mrs));
            Assert.That(getClient.RV, Is.Not.EqualTo(getClient2.RV));

            var deleteResult = encryptionRepo.Delete(insertedParty);
            Assert.That(deleteResult, Is.False);

            deleteResult = encryptionRepo.Delete(getClient2);
            Assert.That(deleteResult, Is.True);

            var getClient3 = encryptionRepo.GetParty(insertedParty.Id);

            Assert.That(getClient3, Is.Null);
        }

    }
}

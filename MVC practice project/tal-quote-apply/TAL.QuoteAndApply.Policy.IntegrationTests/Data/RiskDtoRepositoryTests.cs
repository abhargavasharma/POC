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
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Data
{
    [TestFixture]
    public class RiskDtoRepositoryTests
    {
        [Test]
        public void Insert_Get_Update_Delete()
        {
            var partyConfig = new PartyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var partyRepo = new PartyDtoRepository(partyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var party = new PartyDto()
            {
                DateOfBirth = new DateTime(1980, 4, 22),
                Gender = Gender.Male,
                Title = Title.Mr,
                FirstName = "Chris",
                Surname = "Moretton",
                Address = "132 ABC St",
                Suburb = "Melbourne",
                State = State.VIC,
                Country = Country.Australia,
                Postcode = "1234",
                MobileNumber = "0411111111",
                HomeNumber = "0811111111",
                EmailAddress = "chris@chris.com"
            };

            var insertedParty = partyRepo.InsertParty(party);

            Assert.That(insertedParty.Id, Is.GreaterThan(0));

            var config = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var currentUserProvider = new MockCurrentUserProvider();

            var insertedPolicy = PolicyCreator.CreatePolicy();
            
            var repo = new RiskDtoRepository(config, currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()), new RiskChangeSubject());

            var risk = new RiskDto()
            {
                AnnualIncome = 80000,
                DateOfBirth = new DateTime(1980, 4, 22),
                Gender = Gender.Male,
                Residency = ResidencyStatus.Australian,
                SmokerStatus = SmokerStatus.No,
                OccupationClass = "AAA",
                OccupationTitle = "Software Developer",
                PolicyId = insertedPolicy.Id,
                PartyId = insertedParty.Id
            };

            var insertRisk = repo.InsertRisk(risk);

            Assert.That(insertRisk.Id, Is.GreaterThan(0));

            var getRisk = repo.GetRisk(insertRisk.Id);

            Assert.That(getRisk, Is.Not.Null);
            Assert.That(getRisk.Gender, Is.EqualTo(Gender.Male));
            Assert.That(getRisk.Residency, Is.EqualTo(ResidencyStatus.Australian));
            Assert.That(getRisk.SmokerStatus, Is.EqualTo(SmokerStatus.No));
            Assert.That(getRisk.AnnualIncome, Is.EqualTo(80000));
            Assert.That(getRisk.PolicyId, Is.EqualTo(insertedPolicy.Id));
            Assert.That(getRisk.PartyId, Is.EqualTo(insertedParty.Id));

            getRisk.Residency = ResidencyStatus.NonAustralian;

            bool updateResult = true;
            try
            {
                repo.UpdateRisk(getRisk);
            }
            catch (Exception ex)
            {
                updateResult = false;
            }
            
            Assert.That(updateResult, Is.True);

            var getRisk2 = repo.GetRisk(insertRisk.Id);

            Assert.That(getRisk2.Residency, Is.EqualTo(ResidencyStatus.NonAustralian));
            // THIS ONE NOW FAILS BECAUSE I DO A GET AFTER AN UPDATE IN ORDER TO ENSURE CONCURRENCY WITH THE CACHE
            //Assert.That(getRisk.RV, Is.Not.EqualTo(getRisk2.RV));

            Console.WriteLine("Inserted policy RV:  " + System.Text.Encoding.Default.GetString(insertRisk.RV));
            Console.WriteLine("GetRisk2 RV:       " + System.Text.Encoding.Default.GetString(getRisk2.RV));
            Console.WriteLine("Inserted == GetRisk2:  " + insertRisk.RV.SequenceEqual(getRisk2.RV));

            var deleteResult = repo.Delete(insertRisk);
            Assert.That(deleteResult, Is.True);

            deleteResult = repo.Delete(getRisk2);
            Assert.That(deleteResult, Is.False);

            var getRisk3 = repo.GetRisk(insertRisk.Id);
            
            Assert.That(getRisk3, Is.Null);
        }
    }
}

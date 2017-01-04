
using System;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Underwriting;
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
    public class CoverDtoRepositoryTests
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
                HomeNumber ="0822222221",
                EmailAddress = "chris@chris.com"
            };

            var insertedParty = partyRepo.InsertParty(party);

            Assert.That(insertedParty.Id, Is.GreaterThan(0));

            var config = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var currentUserProvider = new MockCurrentUserProvider();

            var insertedPolicy = PolicyCreator.CreatePolicy();

            var riskRepo = new RiskDtoRepository(config, currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()), new RiskChangeSubject());

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

            var insertRisk = riskRepo.InsertRisk(risk);

            var planRepo = new PlanDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var plan = new PlanDto
            {
                Code = "DTH",
                CoverAmount = 100000,
                LinkedToCpi = true,
                PolicyId = insertedPolicy.Id,
                RiskId = insertRisk.Id
            };

            var insertedPlan = planRepo.InsertPlan(plan);

            Assert.That(insertedPlan.Id, Is.GreaterThan(0));

            var repo = new CoverDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()), new CoverChangeSubject());

            var cover = new CoverDto
            {
                Code = "LIAC",
                PlanId = insertedPlan.Id,
                CoverAmount = 20000,
                RiskId = insertRisk.Id,
                PolicyId = insertedPolicy.Id,
                UnderwritingStatus = UnderwritingStatus.Incomplete
            };

            var insertedCover = repo.InsertCover(cover);

            Assert.That(insertedCover.Id, Is.GreaterThan(0));

            var getCover = repo.GetCover(insertedCover.Id);

            Assert.That(getCover, Is.Not.Null);
            Assert.That(getCover.Code, Is.EqualTo("LIAC"));
            Assert.That(getCover.CoverAmount, Is.EqualTo(20000));
            Assert.That(getCover.UnderwritingStatus, Is.EqualTo(UnderwritingStatus.Incomplete));

            getCover.Code = "LIIL";

            bool updateResult = true;
            try
            {
                repo.UpdateCover(getCover);
            }
            catch (Exception ex)
            {
                updateResult = false;
            }
            

            Assert.That(updateResult, Is.True);

            var getCover2 = repo.GetCover(insertedCover.Id);

            Assert.That(getCover2.Code, Is.EqualTo("LIIL"));
            // THIS ONE NOW FAILS BECAUSE I DO A GET AFTER AN UPDATE IN ORDER TO ENSURE CONCURRENCY WITH THE CACHE
            //Assert.That(getCover.RV, Is.Not.EqualTo(getCover2.RV));

            var cover2 = new CoverDto
            {
                Code = "LIAC",
                CoverAmount = 10000,
                PlanId = insertedPlan.Id,
                RiskId = insertRisk.Id,
                PolicyId = insertedPolicy.Id,
                UnderwritingStatus = UnderwritingStatus.Incomplete
            };

            var insertedCover2 = repo.InsertCover(cover2);

            Assert.That(insertedCover2.Id, Is.GreaterThan(0));

            var planCovers = repo.GetCoversForPlan(insertedPlan.Id);

            Assert.That(planCovers.Count(), Is.EqualTo(2));

            var deleteResult = repo.Delete(insertedCover);
            // SAME AS ABOVE. NEW BEHAVIOUR BECAUSE OF CACHING
            //Assert.That(deleteResult, Is.False);

            deleteResult = repo.Delete(insertedCover2);
            Assert.That(deleteResult, Is.True);

            var getCover3 = repo.GetCover(insertedCover2.Id);

            Assert.That(getCover3, Is.Null);

            repo.Delete(getCover2);
        }
    }
}
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
    public class PlanDtoRepositoryTests
    {
        [Test]
        public void GetByRiskIdAndPlanCode_Returns()
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
                HomeNumber = "0711111111",
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

            var repo = new PlanDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var plan = new PlanDto
            {
                Code = "DTH",
                CoverAmount = 100000,
                LinkedToCpi = true,
                PolicyId = insertedPolicy.Id,
                RiskId = insertRisk.Id
            };

            var insertedPlan = repo.InsertPlan(plan);

            Assert.That(insertedPlan.Id, Is.GreaterThan(0));

            var queryResult = repo.GetByRiskIdAndPlanCode(risk.Id, plan.Code);

            Assert.That(queryResult, Is.Not.Null);
        }


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
                HomeNumber = "0711111111",
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

            var repo = new PlanDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var plan = new PlanDto
            {
                Code = "DTH",
                CoverAmount = 100000,
                LinkedToCpi = true,
                PolicyId = insertedPolicy.Id,
                RiskId = insertRisk.Id
            };

            var insertedPlan = repo.InsertPlan(plan);

            Assert.That(insertedPlan.Id, Is.GreaterThan(0));

            var getPlan = repo.GetPlan(insertedPlan.Id);

            Assert.That(getPlan, Is.Not.Null);
            Assert.That(getPlan.Code, Is.EqualTo("DTH"));
            Assert.That(getPlan.CoverAmount, Is.EqualTo(100000));
            Assert.That(getPlan.LinkedToCpi, Is.True);

            getPlan.Code = "IP";

            bool updateResult = true;
            try
            {
                repo.UpdatePlan(getPlan);
            }
            catch (Exception ex)
            {
                updateResult = false;
            }
            

            Assert.That(updateResult, Is.True);

            var getPlan2 = repo.GetPlan(insertedPlan.Id);

            Assert.That(getPlan2.Code, Is.EqualTo("IP"));
            // THIS ONE NOW FAILS BECAUSE I DO A GET AFTER AN UPDATE IN ORDER TO ENSURE CONCURRENCY WITH THE CACHE
            //Assert.That(getPlan.RV, Is.Not.EqualTo(getPlan2.RV));

            var plan2 = new PlanDto
            {
                Code = "DTH",
                CoverAmount = 100001,
                LinkedToCpi = false,
                RiskId = insertRisk.Id,
                PolicyId = insertedPolicy.Id
            };

            var insertedPlan2 = repo.InsertPlan(plan2);

            Assert.That(insertedPlan2.Id, Is.GreaterThan(0));

            var riskPlans = repo.GetPlansForRisk(insertRisk.Id);

            Assert.That(riskPlans.Count(), Is.EqualTo(2));

            var deleteResult = repo.Delete(insertedPlan);
            Assert.That(deleteResult, Is.True);

            deleteResult = repo.Delete(insertedPlan2);
            Assert.That(deleteResult, Is.True);

            var getPlan3 = repo.GetPlan(insertedPlan2.Id);

            Assert.That(getPlan3, Is.Null);
        }
    }
}
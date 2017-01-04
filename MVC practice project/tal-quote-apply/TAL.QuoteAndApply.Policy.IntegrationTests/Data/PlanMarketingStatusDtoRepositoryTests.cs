using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
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
    public class PlanMarketingStatusDtoRepositoryTests
    {
        [Test]
        public void Insert_Get_Update_GetByPlanId()
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

            var policyConfig = new PolicyConfigurationProvider();

            var planMarketingStatusRepo = new PlanMarketingStatusDtoRepository(policyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var planMarketingStatus = new PlanMarketingStatusDto()
            {
                PlanId = insertedPlan.Id,
                MarketingStatusId = MarketingStatus.Accept,
            };

            var insertedPlanMarketingStatus = planMarketingStatusRepo.InsertPlanMarketingStatus(planMarketingStatus);

            Assert.That(insertedPlanMarketingStatus.Id, Is.GreaterThan(0));
            Assert.That(insertedPlanMarketingStatus.PlanId, Is.EqualTo(insertedPlan.Id));
            Assert.That(insertedPlanMarketingStatus.MarketingStatusId, Is.EqualTo(planMarketingStatus.MarketingStatusId));

            var getByIdMarketingStatus = planMarketingStatusRepo.GetPlanMarketingStatus(insertedPlanMarketingStatus.Id);

            Assert.That(getByIdMarketingStatus.Id, Is.GreaterThan(0));
            Assert.That(getByIdMarketingStatus.PlanId, Is.EqualTo(insertedPlan.Id));
            Assert.That(getByIdMarketingStatus.MarketingStatusId, Is.EqualTo(planMarketingStatus.MarketingStatusId));

            var getPlanMarketingStatusByPlanId = planMarketingStatusRepo.GetPlanMarketingStatusByPlanId(insertedPlan.Id);
            getPlanMarketingStatusByPlanId.MarketingStatusId = MarketingStatus.Decline;

            planMarketingStatusRepo.UpdatePlanMarketingStatus(getPlanMarketingStatusByPlanId);

            getPlanMarketingStatusByPlanId = planMarketingStatusRepo.GetPlanMarketingStatusByPlanId(insertedPlan.Id);

            Assert.That(getPlanMarketingStatusByPlanId.Id, Is.GreaterThan(0));
            Assert.That(getPlanMarketingStatusByPlanId.PlanId, Is.EqualTo(insertedPlan.Id));
            Assert.That(getPlanMarketingStatusByPlanId.MarketingStatusId, Is.EqualTo(getPlanMarketingStatusByPlanId.MarketingStatusId));
        }
    }
}
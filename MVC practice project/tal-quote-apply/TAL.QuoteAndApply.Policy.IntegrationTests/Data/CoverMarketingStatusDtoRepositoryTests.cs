using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
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
    public class CoverMarketingStatusDtoRepositoryTests
    {
        [Test]
        public void Insert_Get_Update_GetByCoverId()
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
                HomeNumber = "0822222221",
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

            var policyConfig = new PolicyConfigurationProvider();

            var coverMarketingStatusRepo = new CoverMarketingStatusDtoRepository(policyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var coverMarketingStatus = new CoverMarketingStatusDto()
            {
                CoverId = insertedCover.Id,
                MarketingStatusId = MarketingStatus.Accept
            };

            var insertedCoverMarketingStatus = coverMarketingStatusRepo.InsertCoverMarketingStatus(coverMarketingStatus);

            Assert.That(insertedCoverMarketingStatus.Id, Is.GreaterThan(0));
            Assert.That(insertedCoverMarketingStatus.CoverId, Is.EqualTo(insertedCover.Id));
            Assert.That(insertedCoverMarketingStatus.MarketingStatusId, Is.EqualTo(coverMarketingStatus.MarketingStatusId));

            var getByIdMarketingStatus = coverMarketingStatusRepo.GetCoverMarketingStatus(insertedCoverMarketingStatus.Id);

            Assert.That(getByIdMarketingStatus.Id, Is.GreaterThan(0));
            Assert.That(getByIdMarketingStatus.CoverId, Is.EqualTo(insertedCover.Id));
            Assert.That(getByIdMarketingStatus.MarketingStatusId, Is.EqualTo(coverMarketingStatus.MarketingStatusId));

            var getCoverMarketingStatusByPlanId = coverMarketingStatusRepo.GetCoverMarketingStatusByCoverId(insertedCover.Id);
            getCoverMarketingStatusByPlanId.MarketingStatusId = MarketingStatus.Decline;

            coverMarketingStatusRepo.UpdateCoverMarketingStatus(getCoverMarketingStatusByPlanId);

            getCoverMarketingStatusByPlanId = coverMarketingStatusRepo.GetCoverMarketingStatusByCoverId(insertedCover.Id);

            Assert.That(getCoverMarketingStatusByPlanId.Id, Is.GreaterThan(0));
            Assert.That(getCoverMarketingStatusByPlanId.CoverId, Is.EqualTo(insertedCover.Id));
            Assert.That(getCoverMarketingStatusByPlanId.MarketingStatusId, Is.EqualTo(getCoverMarketingStatusByPlanId.MarketingStatusId));
        }
    }
}
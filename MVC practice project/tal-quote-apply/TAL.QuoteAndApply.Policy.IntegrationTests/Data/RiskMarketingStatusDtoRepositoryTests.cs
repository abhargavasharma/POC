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
    public class RiskMarketingStatusDtoRepositoryTests
    {
        [Test]
        public void Insert_Get_Update_GetByRiskId()
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

            var policyConfig = new PolicyConfigurationProvider();

            var riskMarketingStatusRepo = new RiskMarketingStatusDtoRepository(policyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var riskMarketingStatus = new RiskMarketingStatusDto()
            {
                RiskId = risk.Id,
                MarketingStatusId = MarketingStatus.Accept
            };

            var insertedRiskMarketingStatus = riskMarketingStatusRepo.InsertRiskMarketingStatus(riskMarketingStatus);

            Assert.That(insertedRiskMarketingStatus.Id, Is.GreaterThan(0));
            Assert.That(insertedRiskMarketingStatus.RiskId, Is.EqualTo(insertRisk.Id));
            Assert.That(insertedRiskMarketingStatus.MarketingStatusId, Is.EqualTo(riskMarketingStatus.MarketingStatusId));

            var getByIdMarketingStatus = riskMarketingStatusRepo.GetRiskMarketingStatus(insertedRiskMarketingStatus.Id);

            Assert.That(getByIdMarketingStatus.Id, Is.GreaterThan(0));
            Assert.That(getByIdMarketingStatus.RiskId, Is.EqualTo(insertRisk.Id));
            Assert.That(getByIdMarketingStatus.MarketingStatusId, Is.EqualTo(riskMarketingStatus.MarketingStatusId));

            var getByIdRiskIdMarketingStatus = riskMarketingStatusRepo.GetRiskMarketingStatusByRiskId(risk.Id);
            getByIdRiskIdMarketingStatus.MarketingStatusId = MarketingStatus.Decline;

            riskMarketingStatusRepo.UpdateRiskMarketingStatus(getByIdRiskIdMarketingStatus);

            getByIdRiskIdMarketingStatus = riskMarketingStatusRepo.GetRiskMarketingStatusByRiskId(risk.Id);

            Assert.That(getByIdRiskIdMarketingStatus.Id, Is.GreaterThan(0));
            Assert.That(getByIdRiskIdMarketingStatus.RiskId, Is.EqualTo(insertRisk.Id));
            Assert.That(getByIdRiskIdMarketingStatus.MarketingStatusId, Is.EqualTo(getByIdRiskIdMarketingStatus.MarketingStatusId));
        }
    }
}
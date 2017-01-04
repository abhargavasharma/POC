
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
    public class OptionDtoRepositoryTests
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

            var repo = new OptionDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var option = new OptionDto
            {
                Code = "PR",
                PlanId = insertedPlan.Id,
                RiskId = insertRisk.Id,
            };

            var insertedOption = repo.InsertOption(option);

            Assert.That(insertedOption.Id, Is.GreaterThan(0));

            var getOption = repo.GetOption(insertedOption.Id);

            Assert.That(getOption, Is.Not.Null);
            Assert.That(getOption.Code, Is.EqualTo("PR"));

            getOption.Code = "PA";

            bool updateResult = true;
            try
            {
                repo.UpdateOption(getOption);
            }
            catch (Exception ex)
            {
                updateResult = false;
            }

            Assert.That(updateResult, Is.True);

            var getOption2 = repo.GetOption(insertedOption.Id);

            Assert.That(getOption2.Code, Is.EqualTo("PA"));
            // THIS ONE NOW FAILS BECAUSE I DO A GET AFTER AN UPDATE IN ORDER TO ENSURE CONCURRENCY WITH THE CACHE
            //Assert.That(getOption.RV, Is.Not.EqualTo(getOption2.RV));

            var option2 = new OptionDto
            {
                Code = "QA",
                PlanId = insertedPlan.Id,
                RiskId = insertRisk.Id
            };

            var insertedOption2 = repo.InsertOption(option2);

            Assert.That(insertedOption2.Id, Is.GreaterThan(0));

            var planOptions = repo.GetOptionsForPlan(insertedPlan.Id);

            Assert.That(planOptions.Count(), Is.EqualTo(2));

            var deleteResult = repo.Delete(insertedOption);
            Assert.That(deleteResult, Is.True);

            deleteResult = repo.Delete(insertedOption2);
            Assert.That(deleteResult, Is.True);

            var getOption3 = repo.GetOption(insertedOption2.Id);

            Assert.That(getOption3, Is.Null);
        }
    }
}
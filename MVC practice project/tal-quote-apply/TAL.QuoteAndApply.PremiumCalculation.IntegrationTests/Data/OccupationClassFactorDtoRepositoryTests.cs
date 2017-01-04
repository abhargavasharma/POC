using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.PremiumCalculation.IntegrationTests.Data
{
    [TestFixture]
    public class OccupationClassFactorDtoRepositoryTests
    {
        [Test]
        public void GetOccupationClassFactorByGenderOccupationClassAndPlan_NoMatch_ReturnsNull()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new OccupationClassFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var planCode = Guid.NewGuid().ToString().Substring(0,9);
            var occupationCode = Guid.NewGuid().ToString();
            var gender = Gender.Unknown;

            var result = repo.GetOccupationClassFactorByGenderOccupationClassAndPlan(gender, occupationCode, planCode, 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetOccupationClassFactorByGenderOccupationClassAndPlan_Match_FactorReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new OccupationClassFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var planCode = Guid.NewGuid().ToString().Substring(0, 9);
            var occupationCode = Guid.NewGuid().ToString().Substring(0, 9);
            var gender = Gender.Unknown;
            var factor = 99.99m;

            repo.Insert(new OccupationClassFactorDto
            {
                Gender = gender,
                PlanCode = planCode,
                OccupationClass = occupationCode,
                Factor = factor,
                BrandId = 1
            });

            var lookupResult = repo.GetOccupationClassFactorByGenderOccupationClassAndPlan(gender, occupationCode, planCode, 1);

            Assert.That(lookupResult.Factor, Is.EqualTo(factor));
        }
    }
}

using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.PremiumCalculation.IntegrationTests.Data
{
    [TestFixture]
    public class WaitingPeriodFactorDtoRepositoryTests
    {
        [Test]
        public void GetWaitingPeriodFactorByWaitingPeriod_NoMatch_ReturnsNull()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new WaitingPeriodFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));
            
            var lookupResult = repo.GetWaitingPeriodFactorByWaitingPeriod(int.MinValue, Guid.NewGuid().ToString().Substring(0, 9), 1);

            Assert.That(lookupResult, Is.Null);
        }

        [Test]
        public void GetOccupationClassFactorByGenderOccupationClassAndPlan_Match_FactorReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new WaitingPeriodFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var planCode = Guid.NewGuid().ToString().Substring(0, 10);
            var waitingPeriod = new Random(5000).Next(0, Int32.MaxValue);
            var factor = 99.99m;

            var waitingPeriodFactor = repo.Insert(new WaitingPeriodFactorDto
            {
                WaitingPeriod = waitingPeriod,
                PlanCode = planCode,
                Factor = factor,
                BrandId = 1
            });

            var lookupResult = repo.GetWaitingPeriodFactorByWaitingPeriod(waitingPeriod, planCode, 1);

            Assert.That(lookupResult.Factor, Is.EqualTo(factor));

            repo.Delete(waitingPeriodFactor);
        }
    }
}
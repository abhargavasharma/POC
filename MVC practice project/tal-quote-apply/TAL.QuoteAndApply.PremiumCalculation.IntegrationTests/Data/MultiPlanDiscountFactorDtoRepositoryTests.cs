using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.PremiumCalculation.IntegrationTests.Data
{
    [TestFixture]
    public class MultiPlanDiscountFactorDtoRepositoryTests
    {
        [Test]
        public void GetMultiPlanDiscountFactorForPlanCount_NoMatch_NullReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new MultiPlanDiscountFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));
            
            var result = repo.GetMultiPlanDiscountFactorForPlanCount(int.MaxValue, 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetMultiPlanDiscountFactorForPlanCount_Match_MultiPlanDiscountFactorReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new MultiPlanDiscountFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            // 5 because 1 - 4 may already exist in the DB
            var planCount = new Random().Next(5, Int32.MaxValue);

            var multiPlanDiscountFactor = new MultiPlanDiscountFactorDto { PlanCount = planCount, Factor = .85m,
                BrandId = 1
            };
            repo.Insert(multiPlanDiscountFactor);

            var result = repo.GetMultiPlanDiscountFactorForPlanCount(multiPlanDiscountFactor.PlanCount, multiPlanDiscountFactor.BrandId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Factor, Is.EqualTo(multiPlanDiscountFactor.Factor));

            repo.Delete(multiPlanDiscountFactor);
        }
    }
}
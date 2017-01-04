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
    public class PlanMinimumCoverAmountForMultiPlanDiscountDtoRepositoryTests
    {
        [Test]
        public void GetMinimumCoverAmountForMultiPlanDiscount_NoMatch_NullReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PlanMinimumCoverAmountForMultiPlanDiscountDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var planCode = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

            var result = repo.GetMinimumCoverAmountForMultiPlanDiscount(planCode, 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetMinimumCoverAmountForMultiPlanDiscount_Match_PlanMinimumCoverAmountForMultiPlanDiscountReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PlanMinimumCoverAmountForMultiPlanDiscountDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var planCode = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

            var planMinimumCoverAmountForMultiPlanDiscountDto = new PlanMinimumCoverAmountForMultiPlanDiscountDto { PlanCode = planCode, MinimumCoverAmount = 100000.85m,
                BrandId = 1
            };
            repo.Insert(planMinimumCoverAmountForMultiPlanDiscountDto);

            var result = repo.GetMinimumCoverAmountForMultiPlanDiscount(planMinimumCoverAmountForMultiPlanDiscountDto.PlanCode, 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MinimumCoverAmount, Is.EqualTo(planMinimumCoverAmountForMultiPlanDiscountDto.MinimumCoverAmount));

            repo.Delete(planMinimumCoverAmountForMultiPlanDiscountDto);
        }
    }
}
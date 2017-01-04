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
    public class MultiCoverDiscountFactorDtoRepositoryTests
    {
        [Test]
        public void GetMultiCoverDiscountFactorForPlan_NoMatch_NullReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new MultiCoverDiscountFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var result = repo.GetMultiCoverDiscountFactorForPlan(Guid.NewGuid().ToString().Substring(0,9), 1, Guid.NewGuid().ToString().Substring(0, 9));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetMultiCoverDiscountFactorForPlan_Match_MultiCoverDiscountFactorReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new MultiCoverDiscountFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var multiCoverDiscountFactor = new MultiCoverDiscountFactorDto
            {
                PlanCode = Guid.NewGuid().ToString().Substring(0, 9),
                SelectedCoverCodes = Guid.NewGuid().ToString().Substring(0, 9),
                Factor = -0.25m,
                BrandId = 1
            };
            repo.Insert(multiCoverDiscountFactor);

            var result = repo.GetMultiCoverDiscountFactorForPlan(multiCoverDiscountFactor.PlanCode, multiCoverDiscountFactor.BrandId, multiCoverDiscountFactor.SelectedCoverCodes);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Factor, Is.EqualTo(multiCoverDiscountFactor.Factor));
        }
    }
}
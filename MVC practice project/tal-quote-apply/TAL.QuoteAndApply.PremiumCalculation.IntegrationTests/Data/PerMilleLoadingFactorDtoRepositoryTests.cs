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
    public class PerMilleLoadingFactorDtoRepositoryTests
    {
        [Test]
        public void GetPerMilleLoadingFactorByCoverCode_NoMatchingFactorForCoverCode_ReturnsNull()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PerMilleLoadingFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var coverCode = Guid.NewGuid().ToString();

            var result = repo.GetPerMilleLoadingFactorByCoverCode(coverCode, 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetPerMilleLoadingFactorByCoverCode_MatchingFactorForCoverCode_ReturnsPercentageLoadingFactor()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PerMilleLoadingFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var coverCode = Guid.NewGuid().ToString().Substring(0, 10);
            var factor = 99.99m;

            repo.Insert(new PerMilleLoadingFactorDto { Factor = factor, CoverCode = coverCode,
                BrandId = 1
            });

            var result = repo.GetPerMilleLoadingFactorByCoverCode(coverCode, 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Factor, Is.EqualTo(factor));
            Assert.That(result.CoverCode, Is.EqualTo(coverCode));
        }
    }
}
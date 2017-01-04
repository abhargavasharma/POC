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
    public class CoverDivisionalFactorDtoRepositoryTests
    {
        [Test]
        public void GetCoverDivisionalFactorByCoverCode_UnknownCode_NullReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new CoverDivisionalFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var somethingUnique = DateTime.Now.Ticks + Guid.NewGuid().ToString();

            var result = repo.GetCoverDivisionalFactorByCoverCode(somethingUnique, 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetCoverDivisionalFactorByCoverCode_KnownCode_DivisionalFactorReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new CoverDivisionalFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var somethingUnique = DateTime.Now.Ticks + Guid.NewGuid().ToString();

            var coverDivisionalFactor = new CoverDivisionalFactorDto {CoverCode = somethingUnique.Substring(0,9), DivisionalFactor = 10000,
                BrandId = 1
            };
            repo.Insert(coverDivisionalFactor);
            
            var result = repo.GetCoverDivisionalFactorByCoverCode(coverDivisionalFactor.CoverCode, 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DivisionalFactor, Is.EqualTo(coverDivisionalFactor.DivisionalFactor));

            repo.Delete(coverDivisionalFactor);
        }
    }
}

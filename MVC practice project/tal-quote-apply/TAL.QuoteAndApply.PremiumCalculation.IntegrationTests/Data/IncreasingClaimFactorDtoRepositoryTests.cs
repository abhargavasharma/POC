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
    public class IncreasingClaimFactorDtoRepositoryTests
    {
        [Test]
        public void IncreasingClaimFactorDto_NoMatch_NullReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new IncreasingClaimsFactorDtoRepository(config, new MockCurrentUserProvider(),
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var result = repo.GetIncreasingClaimFactor(Guid.NewGuid().ToString().Substring(0,9), 1, true, 999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void IncreasingClaimFactorDto_Match_IncreasingClaimFactorReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new IncreasingClaimsFactorDtoRepository(config, new MockCurrentUserProvider(),
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var insertObj = new IncreasingClaimsFactorDto
            {
                BenefitPeriod = 10,
                IncreasingClaimsEnabled = true,
                PlanCode = Guid.NewGuid().ToString().Substring(0, 9),
                Factor = 999,
                BrandId = 1
            };

            repo.Insert(insertObj);

            var result = repo.GetIncreasingClaimFactor(insertObj.PlanCode, insertObj.BrandId, insertObj.IncreasingClaimsEnabled, insertObj.BenefitPeriod);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Factor, Is.EqualTo(999));
        }
    }
}
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
    public class IndemnityFactorDtoRepositoryTests
    {
        [Test]
        public void GetIndemnityFactorByPlanCode_NoMatch_NullReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new IndemnityFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var result = repo.GetIndemnityFactorByPlanCode(Guid.NewGuid().ToString().Substring(0, 9), 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetIndemnityFactorByPlanCode_Match_IndemnityFactorByPlanCodeReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new IndemnityFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var indemnityFactorDto = new IndemnityFactorDto { PlanCode = Guid.NewGuid().ToString().Substring(0, 9), Factor = -0.25m,
                BrandId = 1
            };
            repo.Insert(indemnityFactorDto);

            var result = repo.GetIndemnityFactorByPlanCode(indemnityFactorDto.PlanCode, indemnityFactorDto.BrandId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Factor, Is.EqualTo(indemnityFactorDto.Factor));
        }
    }
}
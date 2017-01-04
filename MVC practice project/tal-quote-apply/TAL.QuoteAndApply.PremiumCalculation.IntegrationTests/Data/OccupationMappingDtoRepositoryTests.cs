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
    public class OccupationMappingDtoRepositoryTests
    {
        [Test]
        public void GetOccupationMappingForOccupationClass_NoMatch_NullReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new OccupationMappingDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var occupationCode = Guid.NewGuid().ToString();

            var result = repo.GetOccupationMappingForOccupationClass(occupationCode, 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetOccupationMappingForOccupationClass_Match_FactorReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new OccupationMappingDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var occupationCode = Guid.NewGuid().ToString().Substring(0, 9);
            var occupationGroup = Guid.NewGuid().ToString();

            repo.Insert(new OccupationMappingDto
            {
                OccupationClass = occupationCode,
                OccupationGroup = occupationGroup,
                BrandId = 1
            });

            var lookupResult = repo.GetOccupationMappingForOccupationClass(occupationCode, 1);

            Assert.That(lookupResult.OccupationGroup, Is.EqualTo(occupationGroup));
        }
    }
}
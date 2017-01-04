using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class OccupationDefinitionTypeFactorDtoRepositoryTests
    {
        [Test]
        public void GetOccupationDefinitionTypeFactorForOccupationDefinition_NoMatch_ReturnsNull()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new OccupationDefinitionTypeFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var lookupResult = repo.GetOccupationDefinitionTypeFactorForOccupationDefinition(OccupationDefinition.Unknown, 1);

            Assert.That(lookupResult, Is.Null);
        }

        [Test]
        public void GetOccupationDefinitionTypeFactorForOccupationDefinition_Match_FactorReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new OccupationDefinitionTypeFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var lookupResult = repo.GetOccupationDefinitionTypeFactorForOccupationDefinition(OccupationDefinition.AnyOccupation, 1);

            Assert.That(lookupResult.Factor, Is.EqualTo(1));
        }
    }
}

using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.PremiumCalculation.IntegrationTests.Data
{
    [TestFixture]
    public class ModalFrequencyFactorDtoRepositoryTests
    {
        [Test]
        public void GetModalFrequencyFactorForPremiumFrequency_PremiumFrequencyUnkown_NullReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new ModalFrequencyFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var result = repo.GetModalFrequencyFactorForPremiumFrequency(PremiumFrequency.Unknown, 1);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetModalFrequencyFactorForPremiumFrequency_PremiumFrequenecyKnown_ModalFrequencyFactorReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new ModalFrequencyFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));
            
            var result = repo.GetModalFrequencyFactorForPremiumFrequency(PremiumFrequency.Monthly, 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Factor, Is.EqualTo(1.0m));
        }
    }
}
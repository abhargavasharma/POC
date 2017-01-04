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
    public class DayOneAccidentBaseRateDtoRepositoryTests
    {
        [Test]
        public void GetAccidentBaseRate_NoMatch_NullReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new DayOneAccidentBaseRateDtoRepository(config, new MockCurrentUserProvider(),
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var result = repo.GetAccidentBaseRate(Guid.NewGuid().ToString().Substring(0, 9), Guid.NewGuid().ToString().Substring(0, 9), 1, 99, Gender.Unknown, PremiumType.Unknown, null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAccidentBaseRate_Match_DayOneAccidentBaseRateReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new DayOneAccidentBaseRateDtoRepository(config, new MockCurrentUserProvider(),
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var insertObj = new DayOneAccidentBaseRateDto
            {
                PlanCode = Guid.NewGuid().ToString().Substring(0, 9),
                CoverCode = Guid.NewGuid().ToString().Substring(0, 9),
                Gender = Gender.Unknown,
                PremiumType = PremiumType.Unknown,
                WaitingPeriod = 1000,
                Age = 99,
                BaseRate = 99.87m,
                BrandId = 1
            };

            repo.Insert(insertObj);

            var result = repo.GetAccidentBaseRate(insertObj.PlanCode, insertObj.CoverCode, insertObj.BrandId, insertObj.Age, insertObj.Gender, insertObj.PremiumType, insertObj.WaitingPeriod);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.BaseRate, Is.EqualTo(99.87m));
        }
    }
}

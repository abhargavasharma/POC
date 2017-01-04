using System;
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
    public class LargeSumInsuredDiscountFactorDtoRepositoryTests
    {
        [Test]
        public void GetLargeSumInsuredDiscountForSumInsured__UnknownCode_NullReturned()
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new LargeSumInsuredDiscountFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var planCode = "BLAH";
            var sumInsured = 0;

            var discount = repo.GetLargeSumInsuredDiscountForSumInsured(sumInsured, planCode, 1);

            Assert.That(discount, Is.EqualTo(null));
        }

        [TestCase(0, null)]
        [TestCase(1, 100)]
        [TestCase(2, 100)]
        [TestCase(3, 200)]
        [TestCase(4, 200)]
        [TestCase(5, 300)]
        [TestCase(6, 300)]
        [TestCase(7, null)]
        public void GetLargeSumInsuredDiscountForSumInsured_MatchSumInsured_FactorReturned(decimal sumInsured, object expectedFactor)
        {
            var config = new PremiumCalculationConfigurationProvider();

            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new LargeSumInsuredDiscountFactorDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var planCode = Guid.NewGuid().ToString().Substring(0, 10);

            var lsi1 = repo.Insert(new LargeSumInsuredDiscountFactorDto
            {
                PlanCode = planCode,
                MinSumInsured = 1,
                MaxSumInsured = 2,
                Factor = 100,
                BrandId = 1
            });

            var lsi2 = repo.Insert(new LargeSumInsuredDiscountFactorDto
            {
                PlanCode = planCode,
                MinSumInsured = 3,
                MaxSumInsured = 4,
                Factor = 200,
                BrandId = 1
            });

            var lsi3 = repo.Insert(new LargeSumInsuredDiscountFactorDto
            {
                PlanCode = planCode,
                MinSumInsured = 5,
                MaxSumInsured = 6,
                Factor = 300,
                BrandId = 1
            });

            var lookupResult = repo.GetLargeSumInsuredDiscountForSumInsured(sumInsured, planCode, 1);
            if (expectedFactor == null)
            {
                Assert.That(lookupResult, Is.EqualTo(null));
            }
            else
            {
                Assert.That(lookupResult.Factor, Is.EqualTo(expectedFactor));
            }

            repo.Delete(new[] {lsi1, lsi2, lsi3});
        }


    }
}

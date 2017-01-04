using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.PremiumCalculation.IntegrationTests.Data
{
    [TestFixture, Ignore("Will be refactored as part of next cut of premium calculation")]
    public class CoverBaseRateDtoRepositoryTests { 
    //{
    //    [Test]
    //    public void GetBaseRateForCriteria_IpAccidentCover_BelowMinimumAge_NullReturned()
    //    {
    //        var config = new PremiumCalculationConfigurationProvider();

    //        DbItemClassMapper<DbItem>.RegisterClassMaps();
    //        var repo = new CoverBaseRateDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

    //        var coverBaseRateCriteria = new CoverBaseRateLookupRequest(
    //            "DTH", "DTHAC", 18, Gender.Male, PremiumType.Stepped, true, 1, "WC");

    //        var result = repo.GetBaseRateForCriteria(coverBaseRateCriteria);

    //        Assert.That(result, Is.Null);
    //    }

    //    [Test]
    //    public void GetBaseRateForCriteria_IpAccidentCover_MinAge_RateReturned()
    //    {
    //        var config = new PremiumCalculationConfigurationProvider();

    //        DbItemClassMapper<DbItem>.RegisterClassMaps();
    //        var repo = new CoverBaseRateDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

    //        var coverBaseRateCriteria = new CoverBaseRateLookupRequest(
    //            "DTH", "DTHAC", 19, Gender.Male, PremiumType.Stepped, true,
    //            1, "WC"
    //            );

    //        var baseRateResult = repo.GetBaseRateForCriteria(coverBaseRateCriteria);

    //        Assert.That(baseRateResult.BaseRate, Is.EqualTo(3.004));
    //    }

    //    [Test]
    //    public void GetBaseRateForCriteria_IpIllnessCover_BelowMinimumAge_NullReturned()
    //    {
    //        var config = new PremiumCalculationConfigurationProvider();

    //        DbItemClassMapper<DbItem>.RegisterClassMaps();
    //        var repo = new CoverBaseRateDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

    //        var coverBaseRateCriteria = new CoverBaseRateLookupRequest(
    //            "DTH", "DTHIC", 
    //            18, 
    //            Gender.Male, 
    //            PremiumType.Stepped,
    //            true,
    //            1, 
    //            "WC"
    //            );

    //        var result = repo.GetBaseRateForCriteria(coverBaseRateCriteria);

    //        Assert.That(result, Is.Null);
    //    }

    //    [Test]
    //    public void GetBaseRateForCriteria_IpIllnessCover_MinAge_RateReturned()
    //    {
    //        var config = new PremiumCalculationConfigurationProvider();

    //        DbItemClassMapper<DbItem>.RegisterClassMaps();
    //        var repo = new CoverBaseRateDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

    //        var coverBaseRateCriteria = new CoverBaseRateLookupRequest(
    //            "DTH", "DTHIC",
    //            19, 
    //            Gender.Male, 
    //            PremiumType.Stepped,
    //            true,
    //            1, "WC"
    //            );

    //        var baseRateResult = repo.GetBaseRateForCriteria(coverBaseRateCriteria);

    //        Assert.That(baseRateResult.BaseRate, Is.EqualTo(6.009));
    //    }
    }
}
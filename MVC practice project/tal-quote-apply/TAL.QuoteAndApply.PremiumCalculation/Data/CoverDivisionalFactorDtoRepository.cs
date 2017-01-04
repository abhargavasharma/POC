using System;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Data
{
    public interface ICoverDivisionalFactorDtoRepository
    {
        CoverDivisionalFactorDto GetCoverDivisionalFactorByCoverCode(string coverCode, int brandId);
    }

    public class CoverDivisionalFactorDtoRepository : BaseRepository<CoverDivisionalFactorDto>, ICoverDivisionalFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public CoverDivisionalFactorDtoRepository(IPremiumCalculationConfigurationProvider premiumCalculationConfigurationProvider, ICurrentUserProvider currentUserProvider, 
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper) 
            : base(premiumCalculationConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public CoverDivisionalFactorDto GetCoverDivisionalFactorByCoverCode(string coverCode, int brandId)
        {
            var q = DbItemPredicate<CoverDivisionalFactorDto>
                .Where(factor => factor.BrandId, Op.Eq, brandId)
                .And(factor => factor.CoverCode, Op.Eq, coverCode);

            var results = _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), coverCode,
                () => Query(q));
            
            return results.FirstOrDefault();
        }
    }

   
}

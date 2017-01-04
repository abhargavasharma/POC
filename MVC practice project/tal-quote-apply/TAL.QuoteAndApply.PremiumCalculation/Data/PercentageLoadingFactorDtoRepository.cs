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
    public interface IPercentageLoadingFactorDtoRepository
    {
        PercentageLoadingFactorDto GetPercentageLoadingFactorByCoverCode(string coverCode, int brandId);
    }

    public class PercentageLoadingFactorDtoRepository : BaseRepository<PercentageLoadingFactorDto>, IPercentageLoadingFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public PercentageLoadingFactorDtoRepository(IPremiumCalculationConfigurationProvider premiumCalculationConfigurationProvider,
            ICurrentUserProvider currentUserProvider, IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(premiumCalculationConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public PercentageLoadingFactorDto GetPercentageLoadingFactorByCoverCode(string coverCode, int brandId)
        {
            var q = DbItemPredicate<PercentageLoadingFactorDto>
                .Where(factor => factor.CoverCode, Op.Eq, coverCode)
                .And(factor => factor.BrandId, Op.Eq, brandId);

            return _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q)).FirstOrDefault();
        }
    }
}
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
    public interface IPremiumReliefFactorDtoRepository
    {
        PremiumReliefFactorDto GetPremiumReliefFactor(bool selected, int brandId);
    }

    public class PremiumReliefFactorDtoRepository : BaseRepository<PremiumReliefFactorDto>, IPremiumReliefFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public PremiumReliefFactorDtoRepository(IPremiumCalculationConfigurationProvider setting, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(setting.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public PremiumReliefFactorDto GetPremiumReliefFactor(bool selected, int brandId)
        {
            var q = DbItemPredicate<PremiumReliefFactorDto>
                .Where(factor => factor.Selected, Op.Eq, selected)
                .And(factor => factor.BrandId, Op.Eq, brandId);

            return _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q)).FirstOrDefault();
        }
    }
}
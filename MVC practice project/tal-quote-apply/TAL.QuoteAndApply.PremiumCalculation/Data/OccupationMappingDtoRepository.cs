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
    public interface IOccupationMappingDtoRepository
    {
        OccupationMappingDto GetOccupationMappingForOccupationClass(string occupationClass, int brandId);
    }

    public class OccupationMappingDtoRepository : BaseRepository<OccupationMappingDto>, IOccupationMappingDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public OccupationMappingDtoRepository(IPremiumCalculationConfigurationProvider setting, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(setting.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public OccupationMappingDto GetOccupationMappingForOccupationClass(string occupationClass, int brandId)
        {
            var q = DbItemPredicate<OccupationMappingDto>
                .Where(factor => factor.OccupationClass, Op.Eq, occupationClass)
                .And(factor => factor.BrandId, Op.Eq, brandId);

            return _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q)).FirstOrDefault();
        }
    }
}
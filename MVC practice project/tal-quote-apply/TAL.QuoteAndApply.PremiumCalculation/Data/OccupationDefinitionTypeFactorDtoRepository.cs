using System;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Data
{
    public interface IOccupationDefinitionTypeFactorDtoRepository
    {
        OccupationDefinitionTypeFactorDto GetOccupationDefinitionTypeFactorForOccupationDefinition(
            OccupationDefinition occupationDefinition, int brandId);
    }

    public class OccupationDefinitionTypeFactorDtoRepository : BaseRepository<OccupationDefinitionTypeFactorDto>,
        IOccupationDefinitionTypeFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;


        public OccupationDefinitionTypeFactorDtoRepository(IPremiumCalculationConfigurationProvider configuration, ICurrentUserProvider currentUserProvider, IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper) : 
            base(configuration.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public OccupationDefinitionTypeFactorDto GetOccupationDefinitionTypeFactorForOccupationDefinition(OccupationDefinition occupationDefinition, int brandId)
        {
            var q = DbItemPredicate<OccupationDefinitionTypeFactorDto>
                .Where(factor => factor.OccupationDefinitionType, Op.Eq, occupationDefinition)
                .And(factor => factor.BrandId, Op.Eq, brandId);

            return
                _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q))
                    .FirstOrDefault();
        }
    }
}
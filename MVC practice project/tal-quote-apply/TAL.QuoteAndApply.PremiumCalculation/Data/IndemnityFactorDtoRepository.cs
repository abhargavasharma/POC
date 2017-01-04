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
    public interface IIndemnityFactorDtoRepository
    {
        IndemnityFactorDto GetIndemnityFactorByPlanCode(string planCode, int brandId);
    }

    public class IndemnityFactorDtoRepository : BaseRepository<IndemnityFactorDto>, IIndemnityFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public IndemnityFactorDtoRepository(IPremiumCalculationConfigurationProvider premiumCalculationConfigurationProvider, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(premiumCalculationConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public IndemnityFactorDto GetIndemnityFactorByPlanCode(string planCode, int brandId)
        {
            var q = DbItemPredicate<IndemnityFactorDto>
                .Where(i => i.PlanCode, Op.Eq, planCode)
                .And(i => i.BrandId, Op.Eq, brandId);
            
            var results = _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), planCode, 
                () => Query(q));
            return results.FirstOrDefault();
        }
    }
}
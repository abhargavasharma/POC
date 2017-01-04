using System;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Data
{
    public interface ISmokerFactorDtoRepository
    {
        SmokerFactorDto GetSmokerFactorBySmokerAndPlan(bool smoker, string planCode, int brandId);
    }

    public class SmokerFactorDtoRepository : BaseRepository<SmokerFactorDto>, ISmokerFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public SmokerFactorDtoRepository(IPremiumCalculationConfigurationProvider setting, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(setting.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public SmokerFactorDto GetSmokerFactorBySmokerAndPlan(bool smoker, string planCode, int brandId)
        {
            var q = DbItemPredicate<SmokerFactorDto>
                .Where(factor => factor.Smoker, Op.Eq, smoker)
                .And(factor => factor.PlanCode, Op.Eq, planCode)
                .And(factor => factor.BrandId, Op.Eq, brandId);

            return _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q)).FirstOrDefault();
        }
    }
}
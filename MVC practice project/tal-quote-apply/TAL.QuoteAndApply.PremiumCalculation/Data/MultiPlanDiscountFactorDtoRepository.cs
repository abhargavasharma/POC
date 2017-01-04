using System;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Data
{
    public interface IMultiPlanDiscountFactorDtoRepository
    {
        MultiPlanDiscountFactorDto GetMultiPlanDiscountFactorForPlanCount(int planCount, int brandId);
    }

    public class MultiPlanDiscountFactorDtoRepository : BaseRepository<MultiPlanDiscountFactorDto>, IMultiPlanDiscountFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public MultiPlanDiscountFactorDtoRepository(IPremiumCalculationConfigurationProvider premiumCalculationConfigurationProvider, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(premiumCalculationConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public MultiPlanDiscountFactorDto GetMultiPlanDiscountFactorForPlanCount(int planCount, int brandId)
        {
            var q = DbItemPredicate<MultiPlanDiscountFactorDto>
                .Where(factor => factor.PlanCount, Op.Eq, planCount)
                .And(factor => factor.BrandId, Op.Eq, brandId);

            var results = _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), planCount.ToString(), 
                () => Query(q));

            return results.FirstOrDefault();
        }
    }
}
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
    public interface IPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository
    {
        PlanMinimumCoverAmountForMultiPlanDiscountDto GetMinimumCoverAmountForMultiPlanDiscount(string planCode, int brandId);
    }

    public class PlanMinimumCoverAmountForMultiPlanDiscountDtoRepository 
        : BaseRepository<PlanMinimumCoverAmountForMultiPlanDiscountDto>, IPlanMinimumCoverAmountForMultiPlanDiscountDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public PlanMinimumCoverAmountForMultiPlanDiscountDtoRepository(IPremiumCalculationConfigurationProvider premiumCalculationConfigurationProvider, ICurrentUserProvider currentUserProvider, IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper) 
            : base(premiumCalculationConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public PlanMinimumCoverAmountForMultiPlanDiscountDto GetMinimumCoverAmountForMultiPlanDiscount(string planCode, int brandId)
        {
            var q = DbItemPredicate<PlanMinimumCoverAmountForMultiPlanDiscountDto>
                .Where(factor => factor.PlanCode, Op.Eq, planCode)
                .And(factor => factor.BrandId, Op.Eq, brandId);

            return _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q)).FirstOrDefault();
        }
    }
}
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
    public interface IMultiCoverDiscountFactorDtoRepository
    {
        MultiCoverDiscountFactorDto GetMultiCoverDiscountFactorForPlan(string planCode, int brandId, string selectedCoverCodes);
    }

    public class MultiCoverDiscountFactorDtoRepository : BaseRepository<MultiCoverDiscountFactorDto>, IMultiCoverDiscountFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public MultiCoverDiscountFactorDtoRepository(IPremiumCalculationConfigurationProvider premiumCalculationConfigurationProvider, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(premiumCalculationConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public MultiCoverDiscountFactorDto GetMultiCoverDiscountFactorForPlan(string planCode, int brandId, string selectedCoverCodes)
        {
            var q = DbItemPredicate<MultiCoverDiscountFactorDto>
               .Where(factor => factor.PlanCode, Op.Eq, planCode)
               .And(factor => factor.BrandId, Op.Eq, brandId)
               .And(factor => factor.SelectedCoverCodes, Op.Eq, selectedCoverCodes);

            var results = _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, 
                () => Query(q));
            return results.FirstOrDefault();
        }
    }
}
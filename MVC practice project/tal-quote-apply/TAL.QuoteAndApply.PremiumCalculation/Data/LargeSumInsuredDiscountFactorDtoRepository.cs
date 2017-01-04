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
    public interface ILargeSumInsuredDiscountFactorDtoRepository
    {
        LargeSumInsuredDiscountFactorDto GetLargeSumInsuredDiscountForSumInsured(decimal sumInsured, string planCode, int brandId);
    }

    public class LargeSumInsuredDiscountFactorDtoRepository : BaseRepository<LargeSumInsuredDiscountFactorDto>, ILargeSumInsuredDiscountFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public LargeSumInsuredDiscountFactorDtoRepository(IPremiumCalculationConfigurationProvider setting, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(setting.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public LargeSumInsuredDiscountFactorDto GetLargeSumInsuredDiscountForSumInsured(decimal sumInsured, string planCode, int brandId)
        {
            var q = DbItemPredicate<LargeSumInsuredDiscountFactorDto>
                .Where(factor => factor.PlanCode, Op.Eq, planCode)
                .And(factor => factor.BrandId, Op.Eq, brandId)
                .And(factor => factor.MinSumInsured, Op.Le, sumInsured)
                .And(factor => factor.MaxSumInsured, Op.Ge, sumInsured);

            return _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q)).FirstOrDefault();
        }
    }
}
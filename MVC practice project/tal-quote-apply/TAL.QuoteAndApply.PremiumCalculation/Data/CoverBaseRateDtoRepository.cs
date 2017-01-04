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
    public interface ICoverBaseRateDtoRepository
    {
        IBaseRate GetBaseRateForCriteria(ICoverBaseRateCriteria baseRateCriteria);
    }

    public class CoverBaseRateDtoRepository : BaseRepository<CoverBaseRateDto>, ICoverBaseRateDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public CoverBaseRateDtoRepository(IPremiumCalculationConfigurationProvider setting, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(setting.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public IBaseRate GetBaseRateForCriteria(ICoverBaseRateCriteria baseRateCriteria)
        {
            var q = DbItemPredicate<CoverBaseRateDto>
                .Where(factor => factor.PlanCode, Op.Eq, baseRateCriteria.PlanCode)
                .And(factor => factor.CoverCode, Op.Eq, baseRateCriteria.CoverCode)
                .And(factor => factor.Age, Op.Eq, baseRateCriteria.Age)
                .And(factor => factor.Gender, Op.Eq, baseRateCriteria.Gender)
                .And(factor => factor.PremiumType, Op.Eq, baseRateCriteria.PremiumType)
                .And(factor => factor.OccupationGroup, Op.Eq, baseRateCriteria.OccupationGroup)
                .And(factor => factor.IsSmoker, Op.Eq, baseRateCriteria.IsSmoker)
                .And(factor => factor.BuyBack, Op.Eq, baseRateCriteria.BuyBack)
                .And(factor => factor.WaitingPeriod, Op.Eq, baseRateCriteria.WaitingPeriod)
                .And(factor => factor.BenefitPeriod, Op.Eq, baseRateCriteria.BenefitPeriod)
                .And(factor => factor.BrandId, Op.Eq, baseRateCriteria.BrandId);

            return _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q)).FirstOrDefault();
        }
    }
}
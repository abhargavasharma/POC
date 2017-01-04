using System;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Data
{
    public interface IDayOneAccidentBaseRateDtoRepository
    {
        DayOneAccidentBaseRateDto GetAccidentBaseRate(string planCode, string coverCode, int brandId, int age, Gender gender, PremiumType premiumType, int? waitingPeriod);
    }

    public class DayOneAccidentBaseRateDtoRepository : BaseRepository<DayOneAccidentBaseRateDto>, IDayOneAccidentBaseRateDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public DayOneAccidentBaseRateDtoRepository(IPremiumCalculationConfigurationProvider premiumCalculationConfigurationProvider, ICurrentUserProvider currentUserProvider,
           IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(premiumCalculationConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public DayOneAccidentBaseRateDto GetAccidentBaseRate(string planCode, string coverCode, int brandId, int age, Gender gender, PremiumType premiumType, int? waitingPeriod)
        {
            var q = DbItemPredicate<DayOneAccidentBaseRateDto>
                .Where(d=> d.PlanCode, Op.Eq, planCode)
                .And(d => d.CoverCode, Op.Eq, coverCode)
                .And(d => d.Age, Op.Eq, age)
                .And(d => d.Gender, Op.Eq, gender)
                .And(d => d.PremiumType, Op.Eq, premiumType)
                .And(d => d.WaitingPeriod, Op.Eq, waitingPeriod)
                .And(d => d.BrandId, Op.Eq, brandId);

            return _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q)).FirstOrDefault();
        }
    }
}

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
    public interface IWaitingPeriodFactorDtoRepository
    {
        WaitingPeriodFactorDto GetWaitingPeriodFactorByWaitingPeriod(int? waitingPeriod, string planCode, int brandId);
    }

    public class WaitingPeriodFactorDtoRepository : BaseRepository<WaitingPeriodFactorDto>,
        IWaitingPeriodFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public WaitingPeriodFactorDtoRepository(IPremiumCalculationConfigurationProvider setting,
            ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService,
            ICachingWrapper cachingWrapper)
            : base(setting.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public WaitingPeriodFactorDto GetWaitingPeriodFactorByWaitingPeriod(int? waitingPeriod, string planCode, int brandId)
        {
            var q = DbItemPredicate<WaitingPeriodFactorDto>
                .Where(factor => factor.PlanCode, Op.Eq, planCode)
                .And(factor => factor.WaitingPeriod, Op.Eq, waitingPeriod)
                .And(factor => factor.BrandId, Op.Eq, brandId);

            return
                _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q))
                    .FirstOrDefault();
        }
    }
}
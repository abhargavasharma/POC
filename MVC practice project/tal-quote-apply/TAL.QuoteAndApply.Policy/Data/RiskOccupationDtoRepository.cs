using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IRiskOccupationDtoRepository
    {
        RiskOccupationDto GetForRisk(int riskId, bool ignoreCache = false);
        void UpdateOccupationRisk(RiskOccupationDto riskOccupationDto);
        RiskOccupationDto Insert(RiskOccupationDto occupationDto);
    }

    public class RiskOccupationDtoRepository : BaseRepository<RiskOccupationDto>, IRiskOccupationDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public RiskOccupationDtoRepository(IPolicyConfigurationProvider settings,
            ICurrentUserProvider currentUserProvider, IDataLayerExceptionFactory dataLayerExceptionFactory,
            IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public RiskOccupationDto GetForRisk(int riskId, bool ignoreCache = false)
        {
            var query = DbItemPredicate<RiskOccupationDto>.Where(r => r.RiskId, Op.Eq, riskId);
            if (ignoreCache)
            {
                return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), query.PredicateKey, Query(query).FirstOrDefault());
            }
            return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"RiskId-{riskId}", () => Query(query).FirstOrDefault());
        }

        public void UpdateOccupationRisk(RiskOccupationDto riskOccupationDto)
        {
            Update(riskOccupationDto);
            var updatedDto = Get(riskOccupationDto.Id);
            riskOccupationDto.RV = updatedDto.RV;
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"RiskId-{riskOccupationDto.RiskId}", riskOccupationDto);
        }
    }
}
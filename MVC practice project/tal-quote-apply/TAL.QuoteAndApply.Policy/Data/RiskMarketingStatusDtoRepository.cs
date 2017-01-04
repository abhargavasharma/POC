using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IRiskMarketingStatusDtoRepository
    {
        RiskMarketingStatusDto InsertRiskMarketingStatus(RiskMarketingStatusDto riskMarketingStatus);
        RiskMarketingStatusDto GetRiskMarketingStatus(int id);
        RiskMarketingStatusDto GetRiskMarketingStatusByRiskId(int riskId);
        void UpdateRiskMarketingStatus(RiskMarketingStatusDto riskMarketingStatus);
    }

    public class RiskMarketingStatusDtoRepository : BaseRepository<RiskMarketingStatusDto>, IRiskMarketingStatusDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public RiskMarketingStatusDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public RiskMarketingStatusDto InsertRiskMarketingStatus(RiskMarketingStatusDto riskMarketingStatus)
        {
            var newDto = Insert(riskMarketingStatus);
            return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"RiskMarketingStatusId-{riskMarketingStatus.Id}", newDto);
        }

        public RiskMarketingStatusDto GetRiskMarketingStatus(int id)
        {
            return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"RiskMarketingStatusId-{id}", () => Get(id));
        }

        public RiskMarketingStatusDto GetRiskMarketingStatusByRiskId(int riskId)
        {
            var riskMarketingStatuses = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"RiskId-{riskId}", 
                () => Where(riskMarketingStatus => riskMarketingStatus.RiskId, Op.Eq, riskId));
            return riskMarketingStatuses.Select(m => GetRiskMarketingStatus(m.Id)).FirstOrDefault();
        }

        public void UpdateRiskMarketingStatus(RiskMarketingStatusDto riskMarketingStatus)
        {
            Update(riskMarketingStatus);
            var updatedDto = Get(riskMarketingStatus.Id);
            riskMarketingStatus.RV = updatedDto.RV;
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"RiskMarketingStatusId-{riskMarketingStatus.Id}", riskMarketingStatus);
        }
    }

}

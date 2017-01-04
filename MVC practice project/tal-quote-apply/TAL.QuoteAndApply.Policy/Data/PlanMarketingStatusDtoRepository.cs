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
    public interface IPlanMarketingStatusDtoRepository
    {
        PlanMarketingStatusDto InsertPlanMarketingStatus(PlanMarketingStatusDto planMarketingStatus);
        PlanMarketingStatusDto GetPlanMarketingStatus(int id);
        PlanMarketingStatusDto GetPlanMarketingStatusByPlanId(int planId);
        void UpdatePlanMarketingStatus(PlanMarketingStatusDto planMarketingStatus);
    }

    public class PlanMarketingStatusDtoRepository : BaseRepository<PlanMarketingStatusDto>, IPlanMarketingStatusDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public PlanMarketingStatusDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public PlanMarketingStatusDto InsertPlanMarketingStatus(PlanMarketingStatusDto planMarketingStatus)
        {
            var newDto = Insert(planMarketingStatus);
            return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"PlanMarketingStatusId-{planMarketingStatus.Id}", newDto);
        }

        public PlanMarketingStatusDto GetPlanMarketingStatus(int id)
        {
            return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"PlanMarketingStatusId-{id}", () => Get(id));
        }

        public PlanMarketingStatusDto GetPlanMarketingStatusByPlanId(int planId)
        {
            var planMarketingStatuses = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"PlanId-{planId}", 
                () => Where(planMarketingStatus => planMarketingStatus.PlanId, Op.Eq, planId));
            return planMarketingStatuses.Select(m => GetPlanMarketingStatus(m.Id)).FirstOrDefault();
        }

        public void UpdatePlanMarketingStatus(PlanMarketingStatusDto planMarketingStatus)
        {
            Update(planMarketingStatus);
            var updatedDto = Get(planMarketingStatus.Id);
            planMarketingStatus.RV = updatedDto.RV;
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"PlanMarketingStatusId-{planMarketingStatus.Id}", planMarketingStatus);
        }
    }
}

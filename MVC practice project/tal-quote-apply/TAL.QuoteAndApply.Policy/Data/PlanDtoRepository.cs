using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IPlanDtoRepository
    {
        PlanDto InsertPlan(PlanDto plan);
        PlanDto GetPlan(int id);
        PlanDto GetByRiskIdAndPlanCode(int riskId, string planCode);
        void UpdatePlan(PlanDto plan);
        IEnumerable<PlanDto> GetPlansForRisk(int riskId);
    }

    public class PlanDtoRepository : BaseRepository<PlanDto>, IPlanDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public PlanDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public PlanDto InsertPlan(PlanDto plan)
        {
            var newDto = Insert(plan);
            return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"PlanId-{plan.Id}", newDto);
        }

        public PlanDto GetPlan(int id)
        {
            return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"PlanId-{id}", () => Get(id));
        }

        public PlanDto GetByRiskIdAndPlanCode(int riskId, string planCode)
        {
            var q = DbItemPredicate<PlanDto>
                .Where(plan => plan.RiskId, Op.Eq, riskId)
                .And(plan => plan.Code, Op.Eq, planCode);

            return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), q.PredicateKey, () => Query(q))
                .Select(p => GetPlan(p.Id)).FirstOrDefault();
        }

        public void UpdatePlan(PlanDto plan)
        {
            Update(plan);
            var updatedDto = Get(plan.Id);
            plan.RV = updatedDto.RV;
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"PlanId-{plan.Id}", plan);
        }

        public IEnumerable<PlanDto> GetPlansForRisk(int riskId)
        {
            var plans = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"RiskId-{riskId}",
                () => Where(plan => plan.RiskId, Op.Eq, riskId));

            return plans.Select(p => GetPlan(p.Id));
        }

        public new bool Delete(PlanDto plan)
        {
            var result = base.Delete(plan);

            if (result)
            {
                _cachingWrapper.RemoveItem(GetType(), $"PlanId-{plan.Id}");
            }

            return result;
        }
    }

}

using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IPlanService
    {
        IPlan GetById(int id);
        IPlan GetByRiskIdAndPlanCode(int riskId, string planCode);
        IPlan CreatePlan(IPlan planDto);
        IPlan GetParentPlanForPlan(IPlan plan, IEnumerable<IPlan> allPlans);

        IEnumerable<IPlan> GetPlansForRisk(int riskId);
        IEnumerable<IPlan> GetParentPlansFromAllPlans(IEnumerable<IPlan> allPlans);
        IEnumerable<IPlan> GetRidersForParentPlan(IPlan parentPlan, IEnumerable<IPlan> allPlans);

        void UpdatePlan(IPlan planDto);
        void SetPlanSelected(int riskId, string planCode, bool isSelected);
        void SetPremiumTypeForAllPlans(int riskId, PremiumType premiumType);

        bool IsPlanSelected(IPlan plan, IPlan parentPlan);
    }
    public class PlanService : IPlanService
    {
        private readonly IPlanDtoRepository _planDtoRepository;

        public PlanService(IPlanDtoRepository planDtoRepository)
        {
            _planDtoRepository = planDtoRepository;
        }

        public IPlan GetById(int id)
        {
            return _planDtoRepository.GetPlan(id);
        }

        public IPlan GetByRiskIdAndPlanCode(int riskId, string planCode)
        {
            return _planDtoRepository.GetByRiskIdAndPlanCode(riskId, planCode);
        }

        public IPlan CreatePlan(IPlan planDto)
        {
            return _planDtoRepository.InsertPlan((PlanDto)planDto);
        }

        public IPlan GetParentPlanForPlan(IPlan plan, IEnumerable<IPlan> allPlans)
        {
            if (plan.ParentPlanId.HasValue)
            {
                return allPlans.FirstOrDefault(p => p.Id == plan.ParentPlanId.Value);
            }

            return null;
        }

        public void UpdatePlan(IPlan planDto)
        {
            _planDtoRepository.UpdatePlan((PlanDto)planDto);
        }

        public void SetPlanSelected(int riskId, string planCode, bool isSelected)
        {
            var plan = GetByRiskIdAndPlanCode(riskId, planCode);
            plan.Selected = isSelected;
            UpdatePlan(plan);
        }

        public void SetPremiumTypeForAllPlans(int riskId, PremiumType premiumType)
        {
            var allPlans = GetPlansForRisk(riskId);
            foreach (var plan in allPlans)
            {
                plan.PremiumType = premiumType;
                UpdatePlan(plan);
            }
        }

        public bool IsPlanSelected(IPlan plan, IPlan parentPlan)
        {
            var planSelected = plan.Selected;
            if (parentPlan != null)
            {
                planSelected = parentPlan.Selected && planSelected;
            }

            return planSelected;
        }

        public IEnumerable<IPlan> GetPlansForRisk(int riskId)
        {
            return _planDtoRepository.GetPlansForRisk(riskId);
        }

        public IEnumerable<IPlan> GetParentPlansFromAllPlans(IEnumerable<IPlan> allPlans)
        {
            return allPlans.Where(p => p.ParentPlanId == null);
        }

        public IEnumerable<IPlan> GetRidersForParentPlan(IPlan parentPlan, IEnumerable<IPlan> allPlans)
        {
            return allPlans.Where(p => p.ParentPlanId == parentPlan.Id);
        }
    }
}

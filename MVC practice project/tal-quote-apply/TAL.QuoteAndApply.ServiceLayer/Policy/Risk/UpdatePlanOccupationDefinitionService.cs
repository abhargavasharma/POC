using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk
{
    public interface IUpdatePlanOccupationDefinitionService
    {
        void Update(int riskId, bool isTpdAny, bool isTpdOwn);
        void Update(IRisk risk);
    }

    public class UpdatePlanOccupationDefinitionService : IUpdatePlanOccupationDefinitionService
    {
        private readonly IPlanService _planService;
        private readonly IPlanDefinitionProvider _planDefinitionProvider;
        private readonly IProductBrandProvider _productBrandProvider;

        public UpdatePlanOccupationDefinitionService(IPlanService planService, IPlanDefinitionProvider planDefinitionProvider, IProductBrandProvider productBrandProvider)
        {
            _planService = planService;
            _planDefinitionProvider = planDefinitionProvider;
            _productBrandProvider = productBrandProvider;
        }

        public void Update(int riskId, bool isTpdAny, bool isTpdOwn)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForRiskId(riskId);
            var plans = _planService.GetPlansForRisk(riskId);
            foreach (var plan in plans)
            {
                UpdatePlan(plan, brandKey, isTpdAny, isTpdOwn);
            }
        }

        public void Update(IRisk risk)
        {
            Update(risk.Id, risk.IsTpdAny, risk.IsTpdOwn);
        }

        private void UpdatePlan(IPlan plan, string brandKey, bool isTpdAny, bool isTpdOwn)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(plan.Code, brandKey);
            var isOccupationDefinitionAvailabvle =
                planDefinition.Variables?.Contains(PlanVariableDefinitions.OccupationDefinition) ?? false;

            if (isOccupationDefinitionAvailabvle)
            {
                plan.OccupationDefinition = GetOccupationDefinition(plan.OccupationDefinition, isTpdAny, isTpdOwn);
            }

            _planService.UpdatePlan(plan);
        }

        public OccupationDefinition GetOccupationDefinition(OccupationDefinition currentPlanDefinition, bool isTpdAny, bool isTpdOwn)
        {
            if (currentPlanDefinition == OccupationDefinition.Unknown)
            {
                if (isTpdAny)
                {
                    currentPlanDefinition = OccupationDefinition.AnyOccupation;
                }
                else if (isTpdOwn)
                {
                    currentPlanDefinition = OccupationDefinition.OwnOccupation;
                }
            }

            if (currentPlanDefinition == OccupationDefinition.OwnOccupation && !isTpdOwn)
            {
                currentPlanDefinition = OccupationDefinition.AnyOccupation;
            }
            if (currentPlanDefinition == OccupationDefinition.AnyOccupation && !isTpdAny)
            {
                currentPlanDefinition = OccupationDefinition.Unknown;
            }

            return currentPlanDefinition;
        }
    }
}

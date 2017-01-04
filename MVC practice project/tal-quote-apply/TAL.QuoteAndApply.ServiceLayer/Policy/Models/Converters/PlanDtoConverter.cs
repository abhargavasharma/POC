using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Plan.Models.Mappers
{
    public interface IPlanDtoConverter
    {
        PlanDto CreateFrom(PlanStateParam planOptionsParam);
        PlanDto CreateFrom(PlanStateParam planOptionsParam, int parentPlanId);

        IEnumerable<PlanOverviewResult> CreateTo(IEnumerable<IPlan> plans);
    }
    public interface IPlanDtoUpdater
    {
        IPlan UpdateFrom(IPlan plan, PlanStateParam updatedPlanOptionsDetails);
    }
    public class PlanDtoConverter : IPlanDtoConverter, IPlanDtoUpdater
    {
        public PlanDto CreateFrom(PlanStateParam planOptionsParam)
        {
            return new PlanDto
            {
                CoverAmount = planOptionsParam.CoverAmount,
                PolicyId = planOptionsParam.PolicyId, 
                RiskId = planOptionsParam.RiskId,
                Code = planOptionsParam.PlanCode,
                LinkedToCpi = planOptionsParam.LinkedToCpi,
                Selected = planOptionsParam.Selected,
                PremiumType = planOptionsParam.PremiumType,
                PremiumHoliday = planOptionsParam.PremiumHoliday,
                WaitingPeriod = planOptionsParam.WaitingPeriod,
                BenefitPeriod = planOptionsParam.BenefitPeriod,
                OccupationDefinition = planOptionsParam.OccupationDefinition
            };
        }

        public PlanDto CreateFrom(PlanStateParam planOptionsParam, int parentPlanId)
        {
            var planDto = CreateFrom(planOptionsParam);
            planDto.ParentPlanId = parentPlanId;

            return planDto;
        }

        public PlanOverviewResult CreateTo(IPlan plan)
        {
            var planItem = new PlanOverviewResult
            {
                CoverAmount = plan.CoverAmount,
                PolicyId = plan.PolicyId,
                RiskId = plan.RiskId,
                Code = plan.Code,
                LinkedToCpi = plan.LinkedToCpi,
                Selected = plan.Selected,
                PlanId = plan.Id,
                Premium = plan.Premium,
                PremiumType = plan.PremiumType,
                PremiumHoliday = plan.PremiumHoliday,
                ParentPlanId = plan.ParentPlanId,
                WaitingPeriod = plan.WaitingPeriod,
                BenefitPeriod = plan.BenefitPeriod,
                OccupationDefinition = plan.OccupationDefinition
            };

            return planItem;
        }

        public IEnumerable<PlanOverviewResult> CreateTo(IEnumerable<IPlan> plans)
        {
            var planList = plans.Select(CreateTo);
            return planList;
        }

        public IPlan UpdateFrom(IPlan plan, PlanStateParam planOptionsUpdates)
        {
            plan.CoverAmount = planOptionsUpdates.CoverAmount;
            plan.LinkedToCpi = planOptionsUpdates.LinkedToCpi;
            plan.Selected = planOptionsUpdates.Selected;
            plan.PolicyId = planOptionsUpdates.PolicyId;
            plan.RiskId = planOptionsUpdates.RiskId;
            plan.PremiumHoliday = planOptionsUpdates.PremiumHoliday;
            plan.PremiumType = planOptionsUpdates.PremiumType;
            plan.WaitingPeriod = planOptionsUpdates.WaitingPeriod;
            plan.BenefitPeriod = planOptionsUpdates.BenefitPeriod;
            plan.OccupationDefinition = planOptionsUpdates.OccupationDefinition;

            return plan;
        }
    }
}

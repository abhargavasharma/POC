using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters
{
    public interface IPlanResponseToStateParamConverter
    {
        PlanStateParam CreateFrom(PlanResponse planResponse, PlanStateParam planStateParam);
    }

    //This is to convert the plan object saved into a format for validating on the service layer - just for non-active plan validation
    public class PlanResponseToStateParamConverter : IPlanResponseToStateParamConverter
    {
        public PlanStateParam CreateFrom(PlanResponse planResponse, PlanStateParam planStateParam)
        {
            var returnObj = PlanStateParam.BuildPlanStateParam(
                planResponse.Code,
                planStateParam.BrandKey, 
                planResponse.Selected, 
                planStateParam.PolicyId, 
                planStateParam.RiskId, 
                planResponse.LinkedToCpi ?? false, 
                planResponse.CoverAmount, 
                planResponse.PremiumHoliday ?? false, 
                planResponse.PremiumType, 
                planResponse.PlanId, 
                planStateParam.Age, 
                planStateParam.Income,
                planResponse.WaitingPeriod ?? 0, 
                planResponse.BenefitPeriod ?? 0, 
                planResponse.OccupationDefinition,
                planResponse.Riders?.Select(x => CreateFrom(x, planStateParam)) ?? new List<PlanStateParam>(),
                planResponse.Options.Select(x => new OptionsParam(x.Code, x.Selected ?? false)), 
                planStateParam.AllPlans?.Select(x => new PlanIdentityInfo(x.PlanId, x.PlanCode, x.Selected)) ?? new List<PlanIdentityInfo>(), 
                planResponse.Covers.Where(x => x.Selected).Select(y => y.Code));
            return returnObj;
        }
    }
}



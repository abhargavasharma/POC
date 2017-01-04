using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPlanOptionsParamService
    {
        PlanStateParam BuildFrom(int policyId, string brandKey, int riskId, PlanResponse planResponse, PlanDefaults planDefaults);
        PlanStateParam BuildFrom(string quoteReferenceNumber, int riskId, PlanResponse planResponse, PlanDefaults planDefaults);
    }

    public class PlanOptionsParamService : IPlanOptionsParamService
    {
        private readonly IPolicyService _policyService;

        public PlanOptionsParamService(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        public PlanStateParam BuildFrom(int policyId, string brandKey, int riskId, PlanResponse planResponse, PlanDefaults planDefaults)
        {
            return PlanStateParam.BuildBasicPlanStateParam(planResponse.Code, brandKey, planDefaults.Selected, policyId, riskId,
                planResponse.LinkedToCpi, planResponse.CoverAmount, planResponse.PremiumHoliday, 
                planDefaults.PremiumType, planResponse.PlanId, planDefaults.WaitingPeriod, planDefaults.BenefitPeriod, planDefaults.OccupationDefinition);
        }

        public PlanStateParam BuildFrom(string quoteReferenceNumber, int riskId, PlanResponse planResponse, PlanDefaults planDefaults)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            return BuildFrom(policy.Id, policy.BrandKey, riskId, planResponse, planDefaults);
        }
    }
}
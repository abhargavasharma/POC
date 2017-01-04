using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.PremiumCalculation.Calculators;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.PremiumCalculation.Services;

namespace TAL.QuoteAndApply.PremiumCalculation
{
    public interface IPremiumCalculationService
    {
        PremiumCalculationResult Calculate(PremiumCalculationRequest premiumCalculationRequest);
    }

    public class PremiumCalculationService : IPremiumCalculationService
    {
        private readonly IGetCoverCalculatorInputService _getCoverCalculatorInputService;
        private readonly IGetRiskCalculatorInputService _getRiskCalculatorInputService;
        private readonly IGetPlanCalculatorInputService _getPlanCalculatorInput;
        private readonly IGetMultiCoverBlockCalculatorInputService _getMultiCoverBlockCalculatorInputService;
        private readonly IGetLoadingCalculatorInputService _getLoadingCalculatorInputService;
        private readonly IGetDayOneAccidentCalculatorInputService _getDayOneAccidentCalculatorInputService;

        public PremiumCalculationService(IGetRiskCalculatorInputService getRiskCalculatorInputService, 
            IGetPlanCalculatorInputService getPlanCalculatorInput,
            IGetCoverCalculatorInputService getCoverCalculatorInputService, 
            IGetMultiCoverBlockCalculatorInputService getMultiCoverBlockCalculatorInputService, 
            IGetLoadingCalculatorInputService getLoadingCalculatorInputService,
            IGetDayOneAccidentCalculatorInputService getDayOneAccidentCalculatorInputService)
        {
            _getCoverCalculatorInputService = getCoverCalculatorInputService;
            _getMultiCoverBlockCalculatorInputService = getMultiCoverBlockCalculatorInputService;
            _getLoadingCalculatorInputService = getLoadingCalculatorInputService;
            _getDayOneAccidentCalculatorInputService = getDayOneAccidentCalculatorInputService;
            _getRiskCalculatorInputService = getRiskCalculatorInputService;
            _getPlanCalculatorInput = getPlanCalculatorInput;
        }

        public PremiumCalculationResult Calculate(PremiumCalculationRequest premiumCalculationRequest)
        {
            var riskPremiumCalculationResults = new List<RiskPremiumCalculationResult>();
            
            foreach (var riskCalculationRequest in premiumCalculationRequest.Risks)
            {
                var planPremiumCalculationResults = CalculatePlans(premiumCalculationRequest, riskCalculationRequest, riskCalculationRequest.Plans);

                var riskPremiumAndDiscount = CalculatePremiumForRisk(riskCalculationRequest, planPremiumCalculationResults);

                riskPremiumCalculationResults.Add(new RiskPremiumCalculationResult(riskCalculationRequest.RiskId, riskPremiumAndDiscount.Premium, riskPremiumAndDiscount.Discount, planPremiumCalculationResults));
            }
            return new PremiumCalculationResult(riskPremiumCalculationResults);
        }

        private IReadOnlyList<PlanPremiumCalculationResult> CalculatePlans(PremiumCalculationRequest premiumCalculationRequest,
            RiskCalculationRequest riskCalculationRequest,
            IReadOnlyList<PlanCalculationRequest> planCalculationRequests)
        {
            var planPremiumCalculationResults = new List<PlanPremiumCalculationResult>();

            foreach (var plan in planCalculationRequests)
            {
                var coverBasePremiumResults = CalculatePremiumForCovers(premiumCalculationRequest, riskCalculationRequest, plan);
                var coverBlockPremiumAndDiscount = CalculateCoverBlockBasePremium(plan, coverBasePremiumResults, premiumCalculationRequest.BrandId);
                var coverPremiumWithLoaingsResults = CalculateCoverLoadings(premiumCalculationRequest, riskCalculationRequest, plan, coverBlockPremiumAndDiscount.Premium, plan.Covers, coverBasePremiumResults);

                var totalPlanPremium = CalculatePlanPremium(plan, riskCalculationRequest.Plans, coverPremiumWithLoaingsResults, premiumCalculationRequest.BrandId);

                planPremiumCalculationResults.Add(new PlanPremiumCalculationResult(plan.PlanCode, totalPlanPremium.Premium, totalPlanPremium.MultiPlanDiscount, totalPlanPremium.MultiPlanDiscountFactor, totalPlanPremium.MultiCoverDiscount, coverPremiumWithLoaingsResults));
            }

            return planPremiumCalculationResults;
        }

        
        private PremiumAndDiscount CalculatePremiumForRisk(RiskCalculationRequest riskCalculationRequest,
            IReadOnlyList<PlanPremiumCalculationResult> planResultsWithDiscounts)
        {

            var input = _getRiskCalculatorInputService.GetRiskCalculatorInput(riskCalculationRequest, planResultsWithDiscounts);

            return RiskCalculator.Calculate(input);
        }

        private PlanPremiumAndDiscounts CalculatePlanPremium(PlanCalculationRequest planCalculationRequest, IEnumerable<IPlanFactors> allPlans, IReadOnlyList<CoverPremiumCalculationResult> coverResults, int brandId)
        {
            var planCalculatorInput = _getPlanCalculatorInput.GetPlanCalculatorInput(planCalculationRequest, allPlans,
                planCalculationRequest.Covers, coverResults, brandId);

            return PlanCalculator.Calculate(planCalculatorInput);
        }

        private IReadOnlyList<CoverPremiumCalculationResult> CalculatePremiumForCovers(PremiumCalculationRequest policyFactors, 
            RiskCalculationRequest riskFactors, PlanCalculationRequest planFactors)
        {
            var coverPremiumCalculationResults = new List<CoverPremiumCalculationResult>();

            foreach (var cover in planFactors.Covers)
            {
                var basePremium = 0m;
                var additionalPremium = 0m;

                if (cover.IsRateableCover)
                {
                    var premiumCalcFactors = new PremiumCalculatorFactors(policyFactors, riskFactors, planFactors, cover);

                    var calculatorInput = _getCoverCalculatorInputService.GetCoverCalculatorInput(premiumCalcFactors, riskFactors.Plans, policyFactors.BrandId);
                    basePremium = CoverCalculator.Calculate(calculatorInput);

                    //this only applies to IP Accident Cover Block if turned on.
                    //run the calculator anyway as it will add zero for other covers
                    var dayOneAccidentCalculatorInput = _getDayOneAccidentCalculatorInputService.GetDayOneAccidentCalculatorInput(premiumCalcFactors);
                    additionalPremium = DayOneAccidentCalculator.Calculate(dayOneAccidentCalculatorInput);
                }

                coverPremiumCalculationResults.Add(new CoverPremiumCalculationResult(cover.CoverCode, basePremium, additionalPremium));
            }
            return coverPremiumCalculationResults;
        }

        private PremiumAndDiscount CalculateCoverBlockBasePremium(PlanCalculationRequest plan, IReadOnlyList<CoverPremiumCalculationResult> coverBasePremiumResults, int brandId)
        {
            var input = _getMultiCoverBlockCalculatorInputService.GetMultiCoverBlockCalculatorInput(plan, plan.Covers, coverBasePremiumResults, brandId);
            return MultiCoverBlockCalculator.Calculate(input);
        }


        private IReadOnlyList<CoverPremiumCalculationResult> CalculateCoverLoadings(
            IPolicyFactors policyFactors, IRiskFactors riskFactors, IPlanFactors planFactors, 
            decimal planBasePremium, 
            IReadOnlyList<CoverCalculationRequest> coverCalculationRequests, 
            IReadOnlyList<CoverPremiumCalculationResult> coverBasePremiumResults)
        {
            var results = new List<CoverPremiumCalculationResult>();

            foreach (var coverCalcRequest in coverCalculationRequests)
            {
                var loadingInput =
                    _getLoadingCalculatorInputService.GetLoadingCalculatorInput(
                        new PremiumCalculatorFactors(policyFactors, riskFactors, planFactors, coverCalcRequest),
                        planBasePremium);

                var loadingPremium = LoadingCalculator.CalculateLoading(loadingInput);

                var matchingBasePremiumResult =
                        coverBasePremiumResults.First(cbpr => cbpr.CoverCode == coverCalcRequest.CoverCode);

                if (loadingPremium.HasValue)
                {
                    CoverPremiumCalculationResult newCoverPremiumResult;
                    if (coverCalcRequest.IsRateableCover)
                    {
                        newCoverPremiumResult = matchingBasePremiumResult.WithLoadingPremium(loadingPremium.Value);
                    }
                    else
                    {
                        newCoverPremiumResult = matchingBasePremiumResult.WithBasePremium(loadingPremium.Value);
                    }
                     
                    results.Add(newCoverPremiumResult);
                }
                else
                {
                    results.Add(matchingBasePremiumResult);
                }
            }

            return results;
        }
    }
}

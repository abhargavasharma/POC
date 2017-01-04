using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.PremiumCalculation.Calculators.Inputs;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Services
{
    public interface IGetRiskCalculatorInputService
    {
        RiskCalculatorInput GetRiskCalculatorInput(RiskCalculationRequest riskCalculationRequest,
            IReadOnlyList<PlanPremiumCalculationResult> planResultsWithDiscounts);
    }

    public class GetRiskCalculatorInputService : IGetRiskCalculatorInputService
    {
        public RiskCalculatorInput GetRiskCalculatorInput(RiskCalculationRequest riskCalculationRequest,
            IReadOnlyList<PlanPremiumCalculationResult> planResultsWithDiscounts)
        {
            var activePlans = riskCalculationRequest.Plans.Where(p => p.Active).ToList();

            var activePlanResultsWithDiscounts =
                planResultsWithDiscounts.Where(pr => activePlans.Select(p => p.PlanCode).Contains(pr.PlanCode));

            return new RiskCalculatorInput(activePlanResultsWithDiscounts.ToList());
        }
    }
}
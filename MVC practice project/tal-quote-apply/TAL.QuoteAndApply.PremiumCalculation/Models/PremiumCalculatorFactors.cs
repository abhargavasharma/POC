namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PremiumCalculatorFactors
    {
        public IPolicyFactors PolicyFactors { get; }
        public IRiskFactors RiskFactors { get; }
        public IPlanFactors PlanFactors { get; }
        public ICoverFactors CoverFactors { get; }

        public PremiumCalculatorFactors(IPolicyFactors policyFactors, IRiskFactors riskFactors, IPlanFactors planFactors, ICoverFactors coverFactors)
        {
            PolicyFactors = policyFactors;
            RiskFactors = riskFactors;
            CoverFactors = coverFactors;
            PlanFactors = planFactors;
        }
    }
}

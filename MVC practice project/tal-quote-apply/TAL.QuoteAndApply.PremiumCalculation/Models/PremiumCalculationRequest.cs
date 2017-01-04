using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PremiumCalculationRequest : IPolicyFactors
    {
        public int BrandId { get; }
        public PremiumFrequency PremiumFrequency { get; }
        public IReadOnlyList<RiskCalculationRequest>  Risks { get; }

        public PremiumCalculationRequest(PremiumFrequency premiumFrequency, IReadOnlyList<RiskCalculationRequest> risks, int brandId)
        {
            PremiumFrequency = premiumFrequency;
            Risks = risks;
            BrandId = brandId;
        }

        public PremiumCalculationRequest Clone()
        {
            return new PremiumCalculationRequest(PremiumFrequency, 
                Risks.Select(r=> new RiskCalculationRequest(r, 
                    r.Plans.Select(p=> new PlanCalculationRequest(p, 
                        p.Covers.Select(c=> new CoverCalculationRequest(c)).ToList()))
                    .ToList()))
                .ToList(), BrandId);
        }
    }
}

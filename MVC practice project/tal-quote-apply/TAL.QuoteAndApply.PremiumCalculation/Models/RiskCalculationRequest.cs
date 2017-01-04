using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class RiskCalculationRequest : IRiskFactors
    {
        public int RiskId { get; }
        public int Age { get; }
        public Gender Gender { get; }
        public bool Smoker { get; }
        public string OccupationClass { get; }
        
        public IReadOnlyList<PlanCalculationRequest> Plans { get; }

        public RiskCalculationRequest(int riskId, int age, Gender gender, bool smoker, string occupationClass, IReadOnlyList<PlanCalculationRequest> plans)
        {
            Age = age;
            Gender = gender;
            Smoker = smoker;
            Plans = plans;
            RiskId = riskId;
            OccupationClass = occupationClass;
        }

        public RiskCalculationRequest(RiskCalculationRequest rcr, IReadOnlyList<PlanCalculationRequest> plans)
            : this(rcr.RiskId, rcr.Age, rcr.Gender, rcr.Smoker, rcr.OccupationClass, plans)
        { }
    }
}
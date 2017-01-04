using System.Collections.Generic;

namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public class PlanDefinition : IAvailability
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Code { get; set; }

        public string RelatedPlanCode { get; set; }
        
        public FeatureRule RuleDefinition { get; set; }

        public int MinimumEntryAgeNextBirthday { get; set; }
        public int MaximumEntryAgeNextBirthday { get; set; }
        public IEnumerable<MaximumEntryAgeNextBirthdayForOccupationClassDefinition> MaximumEntryAgeNextBirthdayForOccupationClass { get; set; }

        public int BenefitExpiryAge { get; set; }
        public int MinimumCoverAmount { get; set; }

        public bool IncludedInMultiPlanDiscount { get; set; }
        public bool UseOccupationDefinition { get; set; }

        public IEnumerable<CoverDefinition> Covers { get; set; }
        public IEnumerable<PlanDefinition> Riders { get; set; }
        public IEnumerable<OptionDefinition> Options { get; set; }
        
        public IEnumerable<AgeRangeCoverAmountDefinition> AgeRangeCoverAmountDefinitions { get; set; }
        public CoverAmountPercentageDefinition CoverAmountPercentageDefinition { get; set; }
        
        public IEnumerable<PlanVariablesDefinition> Variables { get; set; }

    }
}

using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public class ProductDetailsResult
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int MinimumEntryAgeNextBirthday { get; set; }
        public int MaximumEntryAgeNextBirthday { get; set; }
        public int BenefitExpiryAge { get; set; }
        public bool AustralianResidencyRequired { get; set; }
        public int MinimumBenefit { get; set; }
        public int MaximumCoverThresholdCalc { get; set; }
        public int MaximumNumberOfBeneficiaries { get; set; }
        public IEnumerable<PlanResponse> Plans { get; set; }
        public IEnumerable<PremiumTypeResponse> PremiumTypes { get; set; }
        public int? MinimumAnnualIncomeDollars { get; set; }

        public IEnumerable<CreditCardType> AvailableCreditCardTypes { get; set; }
        public bool IsDirectDebitAvailable { get; set; }
        public bool IsSuperannuationAvailable { get; set; }
        public bool IsSmsfAvailable { get; set; }
		
		public bool IsQuoteSaveLoadEnabled { get; set; }
        public string SaveGatePosition { get; set; }

        public ICollection<PolicyOwnerTypeParam> AvailableOwnerTypes { get; set; }
    }

    public class CoverDetailsResult
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class PlanDetailsResult
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public IEnumerable<CoverDetailsResult> Covers { get; set; }
        public IEnumerable<PlanDetailsResult> Riders { get; set; }
        public IEnumerable<OptionDetailsResult> Options { get; set; }
    }

    public class OptionDetailsResult
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}

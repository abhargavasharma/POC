using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public class ProductDefinition
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public IEnumerable<PlanDefinition> Plans { get; set; }
        public IEnumerable<PremiumTypeDefinition> PremiumTypes { get; set; }
        public int? MinimumAnnualIncomeDollars { get; set; }
        public int MinimumEntryAgeNextBirthday { get; set; }
        public int MaximumEntryAgeNextBirthday { get; set; }
        public bool AustralianResidencyRequired { get; set; }
        public int MaximumNumberOfBeneficiaries { get; set; }
        public IEnumerable<IPaymentDefinition> PaymentOptions { get; set; }
        public IEnumerable<PremiumFrequency> AvailablePremiumFrequencies { get; set; }
        public PremiumFrequency DefaultPremiumFrequency { get; set; }
		public bool IsQuoteSaveLoadEnabled { get; set; }
        public SaveGatePosition SaveGatePosition { get; set; }
        public ExternalCustomerRefConfigSettings ExternalCustomerRefConfigSettings { get; set; }
    }
}

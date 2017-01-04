using System.Collections.Generic;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class PremiumTypeOptions
    {
        public string SelectedPremiumType { get; set; }
        public IList<AvailablePremiumType> AvailablePremiumTypes { get; set; }
    }

    public class AvailablePremiumType
    {
        public AvailablePremiumType(string premiumTypeName, string premiumType, decimal total, bool isEnabled)
        {
            PremiumType = premiumType;
            Total = total;
            IsEnabled = isEnabled;
            PremiumTypeName = premiumTypeName;
        }

        public string PremiumType { get; }
        public string PremiumTypeName { get; }
        public bool IsEnabled { get; }
        public decimal Total { get; }
    }
}
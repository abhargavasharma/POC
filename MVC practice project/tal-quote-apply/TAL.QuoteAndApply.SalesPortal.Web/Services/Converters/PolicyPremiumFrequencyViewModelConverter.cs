using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPolicyPremiumFrequencyViewModelConverter
    {
        PolicyFremiumFrequencyViewModel From(PolicyPremiumFrequencyParam policyPremiumFrequencyParam);
    }

    public class PolicyPremiumFrequencyViewModelConverter : IPolicyPremiumFrequencyViewModelConverter
    {
        public PolicyFremiumFrequencyViewModel From(PolicyPremiumFrequencyParam policyPremiumFrequencyParam)
        {
            return new PolicyFremiumFrequencyViewModel
            {
                PremiumFrequency = policyPremiumFrequencyParam.PremiumFrequency.ToFriendlyString(),
                QuoteReferenceNumber = policyPremiumFrequencyParam.QuoteReferenceNumber,
                AvailableFrequencies = policyPremiumFrequencyParam.AvailableFrequencies
            };
        }
    }
}
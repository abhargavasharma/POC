namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class RiskPersonalDetailsResult : RiskPersonalDetailsParam
    {
        public bool IsPersonalDetailsValidForInforce { get; set; }
        public PartyConsentResult PartyConsentResult { get; set; }
    }
}

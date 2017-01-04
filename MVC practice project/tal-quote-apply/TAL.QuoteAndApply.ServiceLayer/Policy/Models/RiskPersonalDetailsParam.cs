namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class RiskPersonalDetailsParam 
    {
        public int RiskId { get; set; }

        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public string Address { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ExternalCustomerReference { get; set; }
        public PartyConsentParam PartyConsentParam { get; set; }
    }
}

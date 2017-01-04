namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class SaveCustomerParam
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int? RiskId { get; set; }
        public bool ExpressConsent { get; set; }
        public bool CallBackSubmitted { get; set; }
    }
}

using System;

namespace TAL.QuoteAndApply.Party.Leads
{
    public class GetLeadResult
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string State { get; set; }
        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }
        public string EmailAddress { get; set; }
        public string AdobeId { get; set; }
        public string Gender { get; set; }
        public string Title { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }
        public string Address { get; set; }
        public string ExternalCustomerReference { get; set; }
        public string Brand { get; set; }
        public GetPreferredCommunicationResult PreferredCommunication { get; set; }
    }

    public class GetPreferredCommunicationResult
    {
       public bool ExpressConsent { get; set; }
       public DateTime? ExpressConsentUpdatedTs { get; set; }
       public bool DncMobile { get; set; }
       public bool DncHomeNumber { get; set; }
       public bool DncEmail { get; set; }
       public bool DncPostalMail { get; set; }
    }
}
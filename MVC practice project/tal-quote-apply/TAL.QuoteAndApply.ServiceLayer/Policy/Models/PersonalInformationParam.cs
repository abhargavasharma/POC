namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PersonalInformationParam
    {
        public string Title { get; private set; }
        public string Firstname { get; private set; }
        public string Surname { get; private set; }
        public string MobileNumber { get; private set; }
        public string HomeNumber { get; private set; }
        public string State { get; private set; }
        public string EmailAddress { get; private set; }
        public long? LeadId { get; private set; }
        public string Address { get; private set; }
        public string Suburb { get; private set; }
        public string Postcode { get; private set; }
        public string ExternalCustomerReference { get; private set; }

        public PersonalInformationParam(string title, string firstname, string surname, string mobileNumber,
            string homeNumber, string state, string emailAddress, long? leadId, string suburb, string address,
            string postcode, string externalCustomerReference = null)
            : this(postcode, state)
        {
            Title = title;
            Firstname = firstname;
            Surname = surname;
            MobileNumber = mobileNumber;
            HomeNumber = homeNumber;
            EmailAddress = emailAddress;
            LeadId = leadId;
            Address = address;
            Suburb = suburb;
            ExternalCustomerReference = externalCustomerReference;
        }

        public PersonalInformationParam(string postcode, string state)
        {
            Postcode = postcode;
            State = state;
        }
    }
}
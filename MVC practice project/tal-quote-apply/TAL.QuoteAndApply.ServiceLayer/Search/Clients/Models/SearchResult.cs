using System;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models
{
    public class SearchResult
    {
        public string QuoteReferenceNumber { get; }
        public decimal Premium { get; }

        public long? LeadId { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public DateTime DateOfBirth { get; }
        public State State { get; }
        public string MobileNumber { get; }
        public string HomeNumber { get; }
        public string EmailAddress { get; }
        public string Gender { get; }
        public string ExternalCustomerReference { get; }
        public string Brand { get; }

        public SearchResult(string quoteReferenceNumber, decimal premium, 
            long? leadId, string firstName, string surname, DateTime dateOfBirth, State state, string mobileNumber, string homeNumber, string emailAddress, string gender, string externalCustomerReference = null, string brand = null)
        {
            QuoteReferenceNumber = quoteReferenceNumber;
            Premium = premium;
            LeadId = leadId;
            FirstName = firstName;
            Surname = surname;
            DateOfBirth = dateOfBirth;
            State = state;
            MobileNumber = mobileNumber;
            HomeNumber = homeNumber;
            EmailAddress = emailAddress;
            Gender = gender;
            ExternalCustomerReference = externalCustomerReference;
            Brand = brand;
        }
    }
}
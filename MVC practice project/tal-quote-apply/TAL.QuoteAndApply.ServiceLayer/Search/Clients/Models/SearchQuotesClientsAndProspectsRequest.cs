using System;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models
{
    public enum SearchType
    {
        QuoteReference,
        LeadId,
        PartyInformation
    }

    public class SearchQuotesClientsAndProspectsRequest
    {
        public SearchType SearchType { get; private set; }
        public string QuoteReferenceNumber { get; private set; }
        public long? LeadId { get; private set; }

        public string FirstName { get; private set; }
        public string Surname { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string MobileNumber { get; private set; }
        public string HomeNumber { get; private set; }
        public string Email { get; private set; }
        public string ExternalCustomerReference { get; private set; }
        public int? BrandId { get; set; }

        public static SearchQuotesClientsAndProspectsRequest QuoteReferenceSearch(string quoteReferenceNumber)
        {
            return new SearchQuotesClientsAndProspectsRequest()
            {
                SearchType = SearchType.QuoteReference,
                QuoteReferenceNumber = quoteReferenceNumber
            };
        }

        public static SearchQuotesClientsAndProspectsRequest LeadIdSearch(long? leadId)
        {
            return new SearchQuotesClientsAndProspectsRequest()
            {
                SearchType = SearchType.LeadId,
                LeadId = leadId
            };
        }

        public static SearchQuotesClientsAndProspectsRequest PartyInfoSearch(string firstName, string surname, DateTime? dob, string mobileNumber, string homeNumber, string email, string externalCustomerReference = null)
        {
            return new SearchQuotesClientsAndProspectsRequest()
            {
                SearchType = SearchType.PartyInformation,
                FirstName = firstName,
                Surname = surname,
                MobileNumber = mobileNumber,
                HomeNumber = homeNumber,
                DateOfBirth = dob,
                Email = email,
                ExternalCustomerReference = externalCustomerReference
            };
        }

    }
}
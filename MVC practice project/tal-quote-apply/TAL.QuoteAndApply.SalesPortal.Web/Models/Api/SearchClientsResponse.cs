using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class SearchClientsResponse
    {
        public SearchClientsResponse()
        {
            Quotes = new List<QuoteSummary>();
        }

        public IEnumerable<QuoteSummary> Quotes { get; set; }
        public SearchResultType ResultType { get; set; }
    }

    public class QuoteSummary
    {
        public string OwnerName { get; set; }
        public string OwnerDateOfBirth { get; set; }
        public string OwnerState { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public string OwnerEmailAddress { get; set; }
        public long? LeadId { get; set; }
        public decimal? Premium { get; set; }
        public string QuoteReferenceNumber { get; set; }
        public string OwnerGender { get; set; }
        public string ExternalCustomerReference { get; set; }
        public string Brand { get; set; }
    }
}
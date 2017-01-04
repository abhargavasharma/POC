using System;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IPolicySearchResult
    {
        string QuoteReference { get; }
        decimal Premium { get; }
        string FirstName { get;}
        string Surname { get; }
        string MobileNumber { get; }
        string HomeNumber { get; }
        string EmailAddress { get; }
        DateTime DateOfBirth { get; }
        State State { get;}
        long? LeadId { get; }
        string ExternalCustomerReference { get; }
        int BrandId { get; }
    }

    public class PolicySearchResultDto : DbItem, IPolicySearchResult
    {
        public string QuoteReference { get; set; }
        public decimal Premium { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateOfBirth { get; set; }
        public State State { get; set; }
        public long? LeadId { get; set; }
        public string ExternalCustomerReference { get; set; }
        public int BrandId { get; set; }
    }
}

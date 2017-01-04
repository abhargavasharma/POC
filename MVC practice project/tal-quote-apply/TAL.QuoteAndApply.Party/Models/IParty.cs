using System;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.Party.Models
{
    public interface IParty
    {
        int Id { get; set; }
        Gender Gender { get; set; }
        DateTime DateOfBirth { get; set; }

        Title Title { get; set; }
        string FirstName { get; set; }
        string Surname { get; set; }

        string Address { get; set; }
        string Suburb { get; set; }
        State State { get; set; }
        string Postcode { get; set; }
        Country Country { get; set; }

        string MobileNumber { get; set; }
        string HomeNumber { get; set; }
        string EmailAddress { get; set; }
        long? LeadId { get; set; }
        string ExternalCustomerReference { get; set; }
    }
}
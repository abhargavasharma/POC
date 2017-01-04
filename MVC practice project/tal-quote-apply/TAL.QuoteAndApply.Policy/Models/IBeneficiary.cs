using System;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IBeneficiary
    {
        string Address { get; set; }
        string Suburb { get; set; }
        DataModel.Personal.State State { get; set; }
        string Postcode { get; set; }
        Country Country { get; set; }
        Gender Gender { get; set; }
        DateTime? DateOfBirth { get; set; }
        int Share { get; set; }
        int? BeneficiaryRelationshipId { get; set; }
        DataModel.Personal.Title Title { get; set; }
        string FirstName { get; set; }
        string Surname { get; set; }
        string PhoneNumber { get; set; }
        string EmailAddress { get; set; }
        int Id { get; set; }
        int RiskId { get; set; }
    }
}
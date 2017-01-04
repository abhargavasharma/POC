using System;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class BeneficiaryDto : DbItem, IAddress, IName, IBeneficiary
    {
        public string Address { get; set; }
        public string Suburb { get; set; }
        public DataModel.Personal.State State { get; set; }
        public string Postcode { get; set; }
        public Country Country { get; set; }
        public Gender Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public int Share { get; set; }
        public int? BeneficiaryRelationshipId { get; set; }

        public DataModel.Personal.Title Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }

        public int RiskId { get; set; }
    }
}
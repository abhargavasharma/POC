using System;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.Party.Models
{
    public class PartyDto : DbItem, IParty, IAddress, IDeletable
    {
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Title Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public string Address { get; set; }
        public string Suburb { get; set; }
        public State State { get; set; }
        public string Postcode { get; set; }
        public Country Country { get; set; }

        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }

        public string EmailAddress { get; set; }
        public long? LeadId { get; set; }
        public string ExternalCustomerReference { get; set; }
		public bool IsDeleted { get; internal set; }
    }
}

using System;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.Tests.Shared.Builders
{
    public class PartyBuilder
    {
        private readonly PartyDto _party;
        private readonly PartyConsentDto _partyConsent;

        public PartyBuilder()
        {
            _party = new PartyDto();
            _partyConsent = new PartyConsentDto();
        }

        public PartyBuilder WithPartyConsentPartyId(int partyId)
        {
            _partyConsent.PartyId = partyId;
            return this;
        }

        public PartyBuilder WithDateOfBirth(DateTime dateOfBirth)
        {
            _party.DateOfBirth = dateOfBirth;
            return this;
        }

        public PartyBuilder WithTitle(Title title)
        {
            _party.Title = title;
            return this;
        }

        public PartyBuilder WithFirstName(string firstName)
        {
            _party.FirstName = firstName;
            return this;
        }

        public PartyBuilder WithSurname(string surname)
        {
            _party.Surname = surname;
            return this;
        }

        public PartyBuilder WithAddress(string address)
        {
            _party.Address = address;
            return this;
        }

        public PartyBuilder WithSuburb(string suburb)
        {
            _party.Suburb = suburb;
            return this;
        }

        public PartyBuilder WithState(State state)
        {
            _party.State = state;
            return this;
        }

        public PartyBuilder WithPostcode(string postCode)
        {
            _party.Postcode = postCode;
            return this;
        }

        public PartyBuilder WithEmailAddress(string emailAddress)
        {
            _party.EmailAddress = emailAddress;
            return this;
        }

        public PartyBuilder WithPhoneNumber(string phoneNumber)
        {
            _party.MobileNumber = phoneNumber;
            _party.HomeNumber = phoneNumber;
            return this;
        }

        public PartyBuilder WithId(int id)
        {
            _party.Id = id;
            return this;
        }

        public PartyBuilder WithLeadsId(int leadsId)
        {
            _party.LeadId = leadsId;
            return this;
        }

        public PartyBuilder WithGender(Gender gender)
        {
            _party.Gender = gender;
            return this;
        }

        public PartyBuilder Default()
        {
            _party.Gender = Gender.Male;
            _party.DateOfBirth = DateTime.Today.AddYears(-30);
            _party.Title = Title.Mr;
            _party.FirstName = "Integration";
            _party.Surname = "Test";
            _party.Address = "123 ABC St";
            _party.Suburb = "Melbourne";
            _party.State = State.VIC;
            _party.Postcode = "3000";
            _party.EmailAddress = "int@test.com.au";
            _party.HomeNumber = "0300000000";
            _party.MobileNumber = "0400000000";
            return this;
        }

        public PartyDto Build()
        {
            return _party;
        }

        public PartyConsentDto BuildPartyConsent(int partyId)
        {
            return new PartyConsentDto(partyId);
        }
    }
}

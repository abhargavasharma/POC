using System;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IPartyDtoConverter
    {
        IParty CreateFrom(PersonalInformationParam personalInformation, RatingFactorsParam ratingFactors);
    }
    public interface IPartyDtoUpdater
    {
        IParty UpdateFrom(IParty party, RiskPersonalDetailsParam updatedPersonalDetails);
        IParty UpdateFrom(IParty party, RatingFactorsParam ratingFactors);
        IParty UpdateFrom(IParty party, PersonalInformationParam personalInformation);
    }
    public class PartyDtoConverter : IPartyDtoConverter, IPartyDtoUpdater
    {
        public IParty CreateFrom(PersonalInformationParam personalInformation, RatingFactorsParam ratingFactors)
        {
            if (personalInformation == null)
            {
                return new PartyDto
                {
                    Gender = ratingFactors.Gender.MapToGender(),
                    DateOfBirth = ratingFactors.DateOfBirth
                };
            }

            return new PartyDto
            {
                Gender = ratingFactors.Gender.MapToGender(),
                DateOfBirth = ratingFactors.DateOfBirth,
                Title = personalInformation.Title.MapToTitle(),
                FirstName = personalInformation.Firstname,
                Surname = personalInformation.Surname,
                MobileNumber = personalInformation.MobileNumber,
                HomeNumber = personalInformation.HomeNumber,
                EmailAddress = personalInformation.EmailAddress,
                State = personalInformation.State.MapToState(),
                Country = Country.Australia,
                LeadId = personalInformation.LeadId,
                Address = personalInformation.Address,
                Postcode = personalInformation.Postcode,
                Suburb = personalInformation.Suburb,
                ExternalCustomerReference = personalInformation.ExternalCustomerReference
            };
        }
        
        public IParty UpdateFrom(IParty party, RiskPersonalDetailsParam updatedPersonalDetails)
        {
            party.Title = updatedPersonalDetails.Title.MapToTitle();
            party.FirstName = updatedPersonalDetails.FirstName;
            party.Surname = updatedPersonalDetails.Surname;
            party.Address = updatedPersonalDetails.Address;
            party.Suburb = updatedPersonalDetails.Suburb;
            party.State = updatedPersonalDetails.State.MapToState();
            party.Postcode = updatedPersonalDetails.Postcode;
            party.MobileNumber = updatedPersonalDetails.MobileNumber;
            party.HomeNumber = updatedPersonalDetails.HomeNumber;
            party.EmailAddress = updatedPersonalDetails.EmailAddress;
            party.ExternalCustomerReference = updatedPersonalDetails.ExternalCustomerReference;

            return party;
        }

        public IParty UpdateFrom(IParty party, RatingFactorsParam ratingFactors)
        {
            party.DateOfBirth = ratingFactors.DateOfBirth;
            party.Gender = ratingFactors.Gender == 'M' ? Gender.Male : Gender.Female;

            return party;
        }

        public IParty UpdateFrom(IParty party, PersonalInformationParam personalInformation)
        {
            party.Title = personalInformation.Title.MapToTitle();
            party.FirstName = personalInformation.Firstname;
            party.Surname = personalInformation.Surname;
            party.Address = personalInformation.Address;
            party.Suburb = personalInformation.Suburb;
            party.State = personalInformation.State.MapToState();
            party.Postcode = personalInformation.Postcode;
            party.MobileNumber = personalInformation.MobileNumber;
            party.HomeNumber = personalInformation.HomeNumber;
            party.EmailAddress = personalInformation.EmailAddress;

            return party;
        }
    }
}

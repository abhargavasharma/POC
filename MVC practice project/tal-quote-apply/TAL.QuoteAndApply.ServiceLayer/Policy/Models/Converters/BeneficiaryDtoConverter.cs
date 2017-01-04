using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IBeneficiaryDtoConverter
    {
        IBeneficiary CreateFrom(RiskBeneficiaryDetailsParam beneficiaryDetails);
        RiskBeneficiaryDetailsParam CreateFrom(IBeneficiary beneficiaryDetails);
        IBeneficiary UpdateFrom(IBeneficiary beneficiary, RiskBeneficiaryDetailsParam beneficiaryDetails);
    }

    public class BeneficiaryDtoConverter : IBeneficiaryDtoConverter
    {
        public IBeneficiary UpdateFrom(IBeneficiary beneficiary, RiskBeneficiaryDetailsParam beneficiaryDetails)
        {
            beneficiary.Address = beneficiaryDetails.Address;
            beneficiary.DateOfBirth = beneficiaryDetails.DateOfBirth;
            beneficiary.EmailAddress = beneficiaryDetails.EmailAddress;
            beneficiary.FirstName = beneficiaryDetails.FirstName;
            beneficiary.PhoneNumber = beneficiaryDetails.PhoneNumber;
            beneficiary.Postcode = beneficiaryDetails.Postcode;
            beneficiary.BeneficiaryRelationshipId = beneficiaryDetails.BeneficiaryRelationshipId;
            beneficiary.Share = beneficiaryDetails.Share;
            beneficiary.State = beneficiaryDetails.State.MapToState();
            beneficiary.RiskId = beneficiaryDetails.RiskId;
            beneficiary.Suburb = beneficiaryDetails.Suburb;
            beneficiary.Surname = beneficiaryDetails.Surname;
            beneficiary.Title = beneficiaryDetails.Title.MapToTitle();
            beneficiary.Id = beneficiaryDetails.Id;
            
            return beneficiary;
        }

        public IBeneficiary CreateFrom(RiskBeneficiaryDetailsParam beneficiaryDetails)
        {
            var retVal = new BeneficiaryDto()
            {
                Address = beneficiaryDetails.Address,
                Country = Country.Australia, // Assume only australia
                DateOfBirth = beneficiaryDetails.DateOfBirth,
                EmailAddress = beneficiaryDetails.EmailAddress,
                FirstName = beneficiaryDetails.FirstName,
                PhoneNumber = beneficiaryDetails.PhoneNumber,
                Postcode = beneficiaryDetails.Postcode,
                BeneficiaryRelationshipId = beneficiaryDetails.BeneficiaryRelationshipId,
                Share = beneficiaryDetails.Share,
                State = beneficiaryDetails.State.MapToState(),
                RiskId = beneficiaryDetails.RiskId,
                Suburb = beneficiaryDetails.Suburb,
                Surname = beneficiaryDetails.Surname,
                Title = beneficiaryDetails.Title.MapToTitle(),
                Id = beneficiaryDetails.Id
            };

            return retVal;
        }

        public RiskBeneficiaryDetailsParam CreateFrom(IBeneficiary beneficiaryDetails)
        {
            var retVal = new RiskBeneficiaryDetailsParam()
            {
                Address = beneficiaryDetails.Address,
                DateOfBirth = beneficiaryDetails.DateOfBirth,
                EmailAddress = beneficiaryDetails.EmailAddress,
                FirstName = beneficiaryDetails.FirstName,
                PhoneNumber = beneficiaryDetails.PhoneNumber,
                Postcode = beneficiaryDetails.Postcode,
                BeneficiaryRelationshipId = beneficiaryDetails.BeneficiaryRelationshipId,
                Share = beneficiaryDetails.Share,
                State = beneficiaryDetails.State.ToString(),
                RiskId = beneficiaryDetails.RiskId,
                Suburb = beneficiaryDetails.Suburb,
                Surname = beneficiaryDetails.Surname,
                Title = beneficiaryDetails.Title.ToString(),
                Id = beneficiaryDetails.Id
            };

            return retVal;
        }
    }
}
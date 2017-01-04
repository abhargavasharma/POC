using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IBeneficiaryDetailsRequestConverter
    {
        RiskBeneficiaryDetailsParam From(BeneficiaryDetailsRequest beneficiary);
        BeneficiaryDetailsRequest From(RiskBeneficiaryDetailsParam beneficiary);
    }

    public class BeneficiaryDetailsRequestConverter : IBeneficiaryDetailsRequestConverter
    {
        public RiskBeneficiaryDetailsParam From(BeneficiaryDetailsRequest beneficiary)
        {
            return new RiskBeneficiaryDetailsParam
            {
                Address = beneficiary.Address,
                DateOfBirth = beneficiary.DateOfBirth.ToDateExcactDdMmYyyy(),
                EmailAddress = beneficiary.EmailAddress,
                FirstName = beneficiary.FirstName,
                PhoneNumber = beneficiary.PhoneNumber,
                BeneficiaryRelationshipId = beneficiary.BeneficiaryRelationshipId,
                Postcode = beneficiary.Postcode,
                Share = int.Parse(beneficiary.Share ?? "0"),
                State = beneficiary.State,
                Suburb = beneficiary.Suburb,
                Surname = beneficiary.Surname,
                Title = beneficiary.Title,
                RiskId = beneficiary.RiskId,
                Id = beneficiary.Id
            };
        }

        public BeneficiaryDetailsRequest From(RiskBeneficiaryDetailsParam beneficiary)
        {
            return new BeneficiaryDetailsRequest
            {
                Address = beneficiary.Address,
                DateOfBirth = beneficiary.DateOfBirth.ToFriendlyString(),
                EmailAddress = beneficiary.EmailAddress,
                FirstName = beneficiary.FirstName,
                PhoneNumber = beneficiary.PhoneNumber,
                BeneficiaryRelationshipId = beneficiary.BeneficiaryRelationshipId,
                Postcode = beneficiary.Postcode,
                Share = beneficiary.Share.ToString(),
                State = beneficiary.State,
                Suburb = beneficiary.Suburb,
                Surname = beneficiary.Surname,
                Title = beneficiary.Title,
                RiskId = beneficiary.RiskId,
                Id = beneficiary.Id
            };
        }
    }
}
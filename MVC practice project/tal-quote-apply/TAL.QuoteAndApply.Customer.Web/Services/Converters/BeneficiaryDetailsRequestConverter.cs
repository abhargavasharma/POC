using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Customer.Web.Extensions;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.Services.Converters
{
    public interface IBeneficiaryDetailsRequestConverter
    {
        RiskBeneficiaryDetailsParam From(BeneficiaryViewModel beneficiary);
        BeneficiaryViewModel From(RiskBeneficiaryDetailsParam beneficiary);
        List<RiskBeneficiaryDetailsParam> FromList(List<BeneficiaryViewModel> beneficiaries);
        List<BeneficiaryViewModel> FromList(List<RiskBeneficiaryDetailsParam> beneficiaries);
    }

    public class BeneficiaryDetailsRequestConverter : IBeneficiaryDetailsRequestConverter
    {
        public RiskBeneficiaryDetailsParam From(BeneficiaryViewModel beneficiary)
        {
            return new RiskBeneficiaryDetailsParam
            {
                Address = beneficiary.Address,
                DateOfBirth = beneficiary.DateOfBirth.ToDateExcactDdMmYyyy(),
                FirstName = beneficiary.FirstName,
                BeneficiaryRelationshipId = beneficiary.BeneficiaryRelationshipId,
                Share = int.Parse(beneficiary.Share ?? "0"),
                Surname = beneficiary.Surname,
                RiskId = beneficiary.RiskId,
                Id = beneficiary.Id,
                PhoneNumber = beneficiary.PhoneNumber,
                Title = beneficiary.Title,
                State = beneficiary.State,
                Postcode = beneficiary.Postcode,
                Suburb = beneficiary.Suburb,
                EmailAddress = beneficiary.EmailAddress
            };
        }

        public BeneficiaryViewModel From(RiskBeneficiaryDetailsParam beneficiary)
        {
            return new BeneficiaryViewModel
            {
                Address = beneficiary.Address,
                DateOfBirth = beneficiary.DateOfBirth.ToFriendlyString(),
                FirstName = beneficiary.FirstName,
                BeneficiaryRelationshipId = beneficiary.BeneficiaryRelationshipId,
                Share = beneficiary.Share.ToString(),
                Surname = beneficiary.Surname,
                RiskId = beneficiary.RiskId,
                Id = beneficiary.Id,
                PhoneNumber = beneficiary.PhoneNumber,
                Title = beneficiary.Title,
                State = beneficiary.State,
                Postcode = beneficiary.Postcode,
                Suburb = beneficiary.Suburb,
                EmailAddress = beneficiary.EmailAddress
            };
        }

        public List<RiskBeneficiaryDetailsParam> FromList(List<BeneficiaryViewModel> beneficiaries)
        {
            return beneficiaries.Select(From).ToList();
        }

        public List<BeneficiaryViewModel> FromList(List<RiskBeneficiaryDetailsParam> beneficiaries)
        {           
            return beneficiaries.Select(From).ToList();
        }
    }
}
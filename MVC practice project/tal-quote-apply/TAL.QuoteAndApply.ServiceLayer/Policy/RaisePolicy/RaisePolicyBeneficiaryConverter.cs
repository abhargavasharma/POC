using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyBeneficiaryConverter
    {
        RaisePolicyBeneficiary From(IBeneficiary cover);
    }

    public class RaisePolicyBeneficiaryConverter : IRaisePolicyBeneficiaryConverter
    {
        public RaisePolicyBeneficiary From(IBeneficiary beneficiary)
        {
            return new RaisePolicyBeneficiary
            {
                Id = beneficiary.Id,
                State = beneficiary.State,
                Surname = beneficiary.Surname,
                Title = beneficiary.Title,
                Gender = beneficiary.Gender,
                RiskId = beneficiary.RiskId,
                Share = beneficiary.Share,
                FirstName = beneficiary.FirstName,
                Address = beneficiary.Address,
                PhoneNumber = beneficiary.PhoneNumber,
                DateOfBirth = beneficiary.DateOfBirth,
                Postcode = beneficiary.Postcode,
                EmailAddress = beneficiary.EmailAddress,
                BeneficiaryRelationshipId = beneficiary.BeneficiaryRelationshipId,
                Suburb = beneficiary.Suburb,
                Country = beneficiary.Country
            };
        }
    }
}
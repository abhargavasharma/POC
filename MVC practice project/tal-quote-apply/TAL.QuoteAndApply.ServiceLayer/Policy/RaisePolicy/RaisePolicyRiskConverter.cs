using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyRiskConverter
    {
        RaisePolicyRisk From(IRisk risk, IParty party);
        RaisePolicyOwner From(IParty party, string fundName, PolicyOwnerType ownerType);
    }

    public class RaisePolicyRiskConverter : IRaisePolicyRiskConverter
    {
        public RaisePolicyRisk From(IRisk risk, IParty party)
        {
            var rpRisk = new RaisePolicyRisk
            {
                Id = risk.Id,
                PolicyId = risk.PolicyId,
                Address = party.Address,
                AnnualIncome = risk.AnnualIncome,
                Country = party.Country,
                DateOfBirth = risk.DateOfBirth,
                EmailAddress = party.EmailAddress,
                FirstName = party.FirstName,
                Gender = party.Gender,
                InterviewId = risk.InterviewId,
                InterviewTemplateVersion = risk.InterviewTemplateVersion,
                LprBeneficiary = risk.LprBeneficiary,
                OccupationClass = risk.OccupationClass,
                OccupationCode = risk.OccupationCode,
                OccupationTitle = risk.OccupationTitle,
                PartyId = risk.PartyId,
                MobileNumber = party.MobileNumber,
                HomeNumber = party.HomeNumber,
                Postcode = party.Postcode,
                Residency = risk.Residency,
                SmokerStatus = risk.SmokerStatus,
                State = party.State,
                Suburb = party.Suburb,
                Surname = party.Surname,
                Title = party.Title,
                Premium = risk.Premium,
                MultiPlanDiscount = risk.MultiPlanDiscount,
                ExternalCustomerReference = party.ExternalCustomerReference
            };

            rpRisk.AssignOccupationProperties(risk);

            return rpRisk;
        }

        public RaisePolicyOwner From(IParty party, string fundName, PolicyOwnerType ownerType)
        {
            var rpOwner = new RaisePolicyOwner
            {
                PartyId = party.Id,
                OwnerType = ownerType,

                FundName = fundName,

                FirstName = party.FirstName,
                Surname = party.Surname,
                Title = party.Title,

                MobileNumber = party.MobileNumber,
                HomeNumber = party.HomeNumber,
                EmailAddress = party.EmailAddress,

                Address = party.Address,
                Country = party.Country,
                Postcode = party.Postcode,
                State = party.State,
                Suburb = party.Suburb,
                ExternalCustomerReference = party.ExternalCustomerReference
            };
            
            return rpOwner;
        }
    }
}
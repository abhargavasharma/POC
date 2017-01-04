using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IRiskOverviewResultConverter
    {
        RiskOverviewResult CreateFrom(IRisk risk, IParty party);
        RiskOverviewResult CreateFrom(IRisk risk, IEnumerable<OccupationDefinition> occupationDefitions, IParty party);
    }

    public class RiskOverviewResultConverter : IRiskOverviewResultConverter
    {
        public RiskOverviewResult CreateFrom(IRisk risk, IParty party)
        {
            var riskReturnVal = new RiskOverviewResult();
            riskReturnVal.RiskId = risk.Id;
            riskReturnVal.InterviewConcurrencyToken = risk.InterviewConcurrencyToken;
            riskReturnVal.Age = risk.DateOfBirth.Age();
            riskReturnVal.AnnualIncome = risk.AnnualIncome;
            riskReturnVal.DateOfBirth = risk.DateOfBirth;

            if (party != null)
            {
                riskReturnVal.FirstName = party.FirstName;
                riskReturnVal.Surname = party.Surname;
            }

            return riskReturnVal;
        }

        public RiskOverviewResult CreateFrom(IRisk risk, IEnumerable<OccupationDefinition> occupationDefitions, IParty party)
        {
            var riskReturnVal = new RiskOverviewResult();
            riskReturnVal.RiskId = risk.Id;
            riskReturnVal.InterviewConcurrencyToken = risk.InterviewConcurrencyToken;
            riskReturnVal.Age = risk.DateOfBirth.Age();
            riskReturnVal.AnnualIncome = risk.AnnualIncome;
            riskReturnVal.DateOfBirth = risk.DateOfBirth;
            riskReturnVal.AvailableDefinitions = occupationDefitions.ToList();
            riskReturnVal.InterviewStatus = risk.InterviewStatus;
            
            if (party != null)
            {
                riskReturnVal.FirstName = party.FirstName;
                riskReturnVal.Surname = party.Surname;
                riskReturnVal.EmailAddress = party.EmailAddress;
                riskReturnVal.ExternalCustomerReference = party.ExternalCustomerReference;
            }

            return riskReturnVal;
        }
    }
}


using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IReferralConverter
    {
        ReferralDetailsResult From(IReferral referralDetailsResult, PolicyOverviewResult policyOverviewResult, IEnumerable<IPlan> plans);
    }

    public class ReferralConverter : IReferralConverter
    {
        public ReferralDetailsResult From(IReferral referralDetailsResult, PolicyOverviewResult policyOverviewResult, IEnumerable<IPlan> plans)
        {
            var owner = policyOverviewResult.OwnerRiskId != null ? policyOverviewResult.Risks.Single(x => x.RiskId == policyOverviewResult.OwnerRiskId.Value) : null;
            return  new ReferralDetailsResult()
            {
                QuoteReferenceNumber = policyOverviewResult.QuoteReferenceNumber,
                AssignedTo = referralDetailsResult.AssignedTo,
                ClientName = string.Join(" ", owner?.FirstName, owner?.Surname),
                CreatedTS = referralDetailsResult.CreatedTS,
                CreatedBy = referralDetailsResult.CreatedBy,
                Plans = plans.Where(s => s.Selected).Select(p => p.Code).ToList(),
                State = GetReferralState(referralDetailsResult),
                ExternalCustomerReference = owner?.ExternalCustomerReference,
                Brand = policyOverviewResult.Brand
            };
        }

        private ReferralState GetReferralState(IReferral referralDetailsResult)
        {
            if (referralDetailsResult.CompletedBy != null && referralDetailsResult.CompletedTS != null)
            {
                return ReferralState.Resolved;
            }
            var returnObj = referralDetailsResult.AssignedTo != null ? ReferralState.InProgress : ReferralState.Unresolved;
            return returnObj;
        }
    }
}

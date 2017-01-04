using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IPolicyOverviewResultConverter
    {
        PolicyOverviewResult CreateFrom(IPolicy policy, int? ownerRiskId);
    }

    public class PolicyOverviewResultConverter : IPolicyOverviewResultConverter
    {
        public PolicyOverviewResult CreateFrom(IPolicy policy, int? ownerRiskId)
        {
            PolicyStatus status;
            Enum.TryParse(policy.Status.ToString(), out status);

            var returnVal = new PolicyOverviewResult
            {
                Status = status,
                PolicyId = policy.Id,
                QuoteReferenceNumber = policy.QuoteReference,
                LastModifiedDateTime = policy.ModifiedTS,
                LastModifiedBy = policy.ModifiedBy,
                Risks = new List<RiskOverviewResult>(),
                OwnerRiskId = ownerRiskId,
                DeclarationAgree = policy.DeclarationAgree,
                Premium = policy.Premium,
                PremiumFrequency = policy.PremiumFrequency,
                Source = policy.Source,
                Brand = policy.BrandKey
            };

            return returnVal;
        }
    }
}

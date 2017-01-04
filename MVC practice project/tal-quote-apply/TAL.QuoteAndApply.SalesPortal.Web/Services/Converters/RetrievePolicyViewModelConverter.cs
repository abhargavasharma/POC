using System.Linq;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IRetrievePolicyViewModelConverter
    {
        RetrievePolicyViewModel CreateFrom(PolicyOverviewResult policyOverviewResult, EditPolicyPermissionsResult editPolicyPermissionsResult);
        RetrievePolicyRiskViewModel CreateFrom(RiskOverviewResult policyOverviewResult);
    }

    public class RetrievePolicyViewModelConverter : IRetrievePolicyViewModelConverter
    {
        private readonly IPolicyStatusViewModelConverter _policyStatusViewModelConverter;

        public RetrievePolicyViewModelConverter(IPolicyStatusViewModelConverter policyStatusViewModelConverter)
        {
            _policyStatusViewModelConverter = policyStatusViewModelConverter;
        }

        public RetrievePolicyViewModel CreateFrom(PolicyOverviewResult policyOverviewResult, EditPolicyPermissionsResult editPolicyPermissionsResult)
        {
            var model = new RetrievePolicyViewModel
            {
                Status = _policyStatusViewModelConverter.From(policyOverviewResult.Status),
                PolicyId = policyOverviewResult.PolicyId,
                QuoteReferenceNumber = policyOverviewResult.QuoteReferenceNumber,
                LastSavedDate = policyOverviewResult.LastModifiedDateTime,
                Risks = policyOverviewResult.Risks.Select(CreateFrom),
                ReadOnly = editPolicyPermissionsResult.ReadOnly,
                UserRole = editPolicyPermissionsResult.Role.ToString(),
                OwnerType = policyOverviewResult.OwnerType.ToString()
            };

            return model;
        }

        public RetrievePolicyRiskViewModel CreateFrom(RiskOverviewResult policyOverviewResult)
        {
            return new RetrievePolicyRiskViewModel
            {
                RiskId = policyOverviewResult.RiskId,
                Age = policyOverviewResult.Age,
                FirstName = policyOverviewResult.FirstName,
                Surname = policyOverviewResult.Surname
            };
        }
    }
}
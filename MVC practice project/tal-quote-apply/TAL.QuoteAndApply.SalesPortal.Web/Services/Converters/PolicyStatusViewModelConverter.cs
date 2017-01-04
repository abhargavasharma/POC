using System;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPolicyStatusViewModelConverter
    {
        PolicyStatusViewModel From(PolicyStatusParam policyStatus);
        ViewModelPolicyStatus From(PolicyStatus policyStatus);
    }

    public class PolicyStatusViewModelConverter : IPolicyStatusViewModelConverter
    {
        public PolicyStatusViewModel From(PolicyStatusParam policyStatus)
        {
            return new PolicyStatusViewModel()
            {
                Status = From(policyStatus.Status),
                QuoteReferenceNumber = policyStatus.QuoteReferenceNumber
            };
        }

        public ViewModelPolicyStatus From(PolicyStatus policyStatus)
        {
            switch (policyStatus)
            {
                case PolicyStatus.RaisedToPolicyAdminSystem:
                    return ViewModelPolicyStatus.RaisedToPolicyAdminSystem;
                case PolicyStatus.ReadyForInforce:
                    return ViewModelPolicyStatus.ReadyForInforce;
                case PolicyStatus.ReferredToUnderwriter:
                    return ViewModelPolicyStatus.ReferredToUnderwriter;
                case PolicyStatus.FailedDuringPolicyAdminSystemLoad:
                    return ViewModelPolicyStatus.FailedDuringPolicyAdminSystemLoad;
                case PolicyStatus.FailedToSendToPolicyAdminSystem:
                    return ViewModelPolicyStatus.FailedToSendToPolicyAdminSystem;
                case PolicyStatus.Inforce:
                    return ViewModelPolicyStatus.Inforce;
                case PolicyStatus.Incomplete:
                default:
                    return ViewModelPolicyStatus.Incomplete;
            }
        }
    }
}
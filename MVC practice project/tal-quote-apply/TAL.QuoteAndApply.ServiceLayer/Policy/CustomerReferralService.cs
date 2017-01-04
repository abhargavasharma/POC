using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface ICustomerReferralService
    {
        void SetPolicyAsCustomerReferral(string quoteReference);
    }

    public class CustomerReferralService : ICustomerReferralService
    {
        private readonly IPolicyInteractionService _policyInteractionService;
        private readonly IPolicyService _policyService;

        public CustomerReferralService(IPolicyInteractionService policyInteractionService, IPolicyService policyService)
        {
            _policyInteractionService = policyInteractionService;
            _policyService = policyService;
        }

        public void SetPolicyAsCustomerReferral(string quoteReference)
        {
            _policyService.UpdatePolicySaveStatus(quoteReference, PolicySaveStatus.LockedOutDueToRefer);
            _policyInteractionService.CustomerReferral(quoteReference);
        }
    }
}
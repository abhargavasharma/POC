using TAL.QuoteAndApply.Policy.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Referral
{
    public interface ICreateReferralService
    {
        CreateReferralResult CreateReferralFor(string quoteReference);
    }

    public class CreateReferralService : ICreateReferralService
    {
        private readonly IPolicyService _policyService;
        private readonly IReferralService _referralService;
        private readonly IPolicyInteractionService _policyInteractionService;

        public CreateReferralService(IPolicyService policyService, IReferralService referralService, IPolicyInteractionService policyInteractionService)
        {
            _policyService = policyService;
            _referralService = referralService;
            _policyInteractionService = policyInteractionService;
        }

        public CreateReferralResult CreateReferralFor(string quoteReference)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);

            var existingReferral = _referralService.GetInprogressReferralForPolicy(policy.Id);

            if (existingReferral != null)
            {
                return CreateReferralResult.ReferralAlreadyExistsForPolicy;
            }
            
            policy = _policyService.UpdatePolicyToReferredToUnderwriter(policy);

            _referralService.CreateReferral(policy.Id);
            _policyInteractionService.PolicyReferredToUnderwriter(quoteReference);

            return CreateReferralResult.Created;
        }
    }
}

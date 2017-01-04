using TAL.QuoteAndApply.Policy.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyDeclarationService
    {
        bool UpdateDeclaration(string quoteReference, bool declarationAgree);
    }

    public class PolicyDeclarationService : IPolicyDeclarationService
    {
        private readonly IPolicyService _policyService;

        public PolicyDeclarationService(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        public bool UpdateDeclaration(string quoteReference, bool declarationAgree)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);
            //Todo - may be worth removing this service and setting declarationAgree bool when we create raisePolicy object for customer portal
            _policyService.UpdatePolicyDeclarationAgree(policy, declarationAgree);
            return declarationAgree;
        }
    }
}

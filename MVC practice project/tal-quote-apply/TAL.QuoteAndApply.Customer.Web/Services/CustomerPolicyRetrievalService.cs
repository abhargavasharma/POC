using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public interface ICustomerPolicyRetrievalService
    {
        RetrieveQuoteResponse RetrieveQuote(string quoteReference, string password);
    }

    public class CustomerPolicyRetrievalService : ICustomerPolicyRetrievalService
    {
        private readonly IQuoteSessionContext _quoteSessionContext;
        private readonly IPolicyRetrievalStatusProvider _policyRetrievalStatusProvider;
        private readonly IPolicyAutoUpdateService _policyAutoUpdateService;
        private readonly IPolicyInteractionService _interactionService;

        public CustomerPolicyRetrievalService(IQuoteSessionContext quoteSessionContext,
            IPolicyRetrievalStatusProvider policyRetrievalStatusProvider,
            IPolicyAutoUpdateService policyAutoUpdateService, IPolicyInteractionService interactionService)
        {
            _quoteSessionContext = quoteSessionContext;
            _policyRetrievalStatusProvider = policyRetrievalStatusProvider;
            _policyAutoUpdateService = policyAutoUpdateService;
            _interactionService = interactionService;
        }

        public RetrieveQuoteResponse RetrieveQuote(string quoteReference, string password)
        {
            var retrievalStatus = _policyRetrievalStatusProvider.GetFrom(quoteReference, password);

            if (retrievalStatus != PolicyRetrievalStatus.CanBeRetrieved)
            {
                switch (retrievalStatus)
                {
                    case PolicyRetrievalStatus.LockedOutDueToRefer:
                    case PolicyRetrievalStatus.AlreadySubmitted:
                        return new RetrieveQuoteResponse("Our records show you have already submitted a quote successfully. To find out more, please call 131 825");

                    case PolicyRetrievalStatus.InvalidPassword:
                    case PolicyRetrievalStatus.QuoteReferenceNotFound:
                    case PolicyRetrievalStatus.NotSaved:
                        return new RetrieveQuoteResponse("Combination of reference number and password are invalid");

                    case PolicyRetrievalStatus.ReferredToUnderwriter:
                        return new RetrieveQuoteResponse("Our records show you have submitted a quote and it is presently being reviewed. To find out more, please call 131 825");

                    default:
                        //Default catch-all. We won't ever get here now but just in case changes are made in the future and we don't account for all messages here
                        return new RetrieveQuoteResponse("Quote cannot be retrieved"); 
                }
            }

            //TODO: Currently doing a blanket re-calculate quote here. Process of honouring of original quote etc not worried about at the moment
            _policyAutoUpdateService.AutoUpdatePlansForEligibililityAndRecalculatePremium(quoteReference);

            _interactionService.PolicyRetrievedByCustomer(quoteReference);
            
            _quoteSessionContext.Set(quoteReference);
            return new RetrieveQuoteResponse();
        }
    }
}
using System.Linq;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Interactions.Models;
using TAL.QuoteAndApply.Interactions.Service;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using PolicySource = TAL.QuoteAndApply.ServiceLayer.Models.PolicySource;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyInteractionService
    {
        void PolicyAccessed (int policyId);
        PolicyInteractionsResult GetInteractions(PolicyInteractionsRequest policyInteractionsRequest);
        void PolicyProgressChanged(int policyId);
        void PolicySavedByCustomer(string quoteReference);
        void PolicyRetrievedByCustomer(string quoteReference);
        void PolicyCreatedByPortal(PolicySource source, string quoteReference);
        void PolicySubmitted(string quoteReference);
        void PolicyReferredToUnderwriter(string quoteReference);
        void PolicyReferralCompletedByUnderwriter(string quoteReference);
        void PolicyNoteAdded(string quoteReference);
        void QuoteSavedEmailSent(string quoteReferenceNumber);
        void CustomerReferral(string quoteReference);
        void QuoteEmailSentFromSalesPortal(string quoteReferenceNumber);
        void ApplicationConfirmationEmailSentFromSalesPortal(string quoteReferenceNumber);
        void CustomerCallbackRequested(string quoteReference);
        void CustomerWebChatRequested(string quoteReference);
    }

    public class PolicyInteractionsService : IPolicyInteractionService
    {
        private readonly IInteractionsService _interactionsService;
        private readonly IPolicyService _policyService;
        private readonly IPolicyInteractionsResultConverter _policyInteractionsResultConverter;

        public PolicyInteractionsService(IInteractionsService interactionsService, IPolicyService policyService, IPolicyInteractionsResultConverter policyInteractionsResultConverter)
        {
            _policyService = policyService;
            _interactionsService = interactionsService;
            _policyInteractionsResultConverter = policyInteractionsResultConverter;
        }

       public void PolicyAccessed(int policyId)
       {
            _interactionsService.CreateInteraction(new PolicyInteractionDto()
            {
                PolicyId = policyId, InteractionType = InteractionType.Quote_Accessed
            });
        }

        public void PolicyProgressChanged(int policyId)
        {
            _interactionsService.CreateInteraction(new PolicyInteractionDto()
            {
                PolicyId = policyId,
                InteractionType = InteractionType.Pipeline_Status_Change
            });
        }

        public void PolicySavedByCustomer(string quoteReference)
        {
            CreateForQuoteReference(quoteReference, InteractionType.Saved_By_Customer);
        }

        public void PolicyRetrievedByCustomer(string quoteReference)
        {
            CreateForQuoteReference(quoteReference, InteractionType.Retrieved_By_Customer);
        }

        public void PolicyCreatedByPortal(PolicySource source, string quoteReference)
        {
            var interactionType = InteractionType.Created_By_Agent;
            if (source == PolicySource.CustomerPortalBuildMyOwn || source == PolicySource.CustomerPortalHelpMeChoose)
            {
                interactionType = InteractionType.Created_By_Customer;
            }
            CreateForQuoteReference(quoteReference, interactionType);
        }

        public void PolicySubmitted(string quoteReference)
        {
            CreateForQuoteReference(quoteReference, InteractionType.Submitted_To_Policy_Admin_System);
        }

        public void PolicyReferredToUnderwriter(string quoteReference)
        {
            CreateForQuoteReference(quoteReference, InteractionType.Referred_To_Underwriter);
        }

        public void PolicyReferralCompletedByUnderwriter(string quoteReference)
        {
            CreateForQuoteReference(quoteReference, InteractionType.Referral_Completed_By_Underwriter);
        }

        public void PolicyNoteAdded(string quoteReference)
        {
            CreateForQuoteReference(quoteReference, InteractionType.Policy_Note_Added);
        }

        public void QuoteSavedEmailSent(string quoteReferenceNumber)
        {
            CreateForQuoteReference(quoteReferenceNumber, InteractionType.Quote_Save_Email_Sent);
        }

        public void CustomerReferral(string quoteReference)
        {
            CreateForQuoteReference(quoteReference, InteractionType.Customer_Submit_Application_Referred);
        }

        public void QuoteEmailSentFromSalesPortal(string quoteReferenceNumber)
        {
            CreateForQuoteReference(quoteReferenceNumber, InteractionType.Quote_Email_Sent_From_Sales_Portal);
        }

        public void ApplicationConfirmationEmailSentFromSalesPortal(string quoteReferenceNumber)
        {
            CreateForQuoteReference(quoteReferenceNumber, InteractionType.Application_Confirmation_Email_Sent);
        }

        public void CustomerCallbackRequested(string quoteReference)
        {
            CreateForQuoteReference(quoteReference, InteractionType.Customer_Callback_Requested);
        }

        public void CustomerWebChatRequested(string quoteReference)
        {
            CreateForQuoteReference(quoteReference, InteractionType.Customer_Webchat_Requested);
        }

        public PolicyInteractionsResult GetInteractions(PolicyInteractionsRequest policyInteractionsRequest)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(policyInteractionsRequest.QuoteReferenceNumber);
            var interactions = _interactionsService.GetInteractionsByPolicyId(policy.Id);
            return new PolicyInteractionsResult(interactions.Select(_policyInteractionsResultConverter.From).ToList());
        }

        private void CreateForQuoteReference(string quoteReference, InteractionType interactionType)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);
            _interactionsService.CreateInteraction(new PolicyInteractionDto()
            {
                PolicyId = policy.Id,
                InteractionType = interactionType
            });
        }
    }
}

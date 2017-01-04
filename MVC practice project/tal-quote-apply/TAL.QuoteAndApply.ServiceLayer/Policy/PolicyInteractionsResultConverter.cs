using TAL.QuoteAndApply.Interactions.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyInteractionsResultConverter
    {
            PolicyInteractions From(IPolicyInteraction policyInteractionResult);
    }
    public class PolicyInteractionsResultConverter : IPolicyInteractionsResultConverter
    {
        public PolicyInteractions From(IPolicyInteraction policyInteractionResult)
        {
            return new PolicyInteractions( policyInteractionResult.CreatedTS, policyInteractionResult.InteractionType, policyInteractionResult.CreatedBy);
        }
    }
}

using System.Linq;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPolicyInteractionsResultConverter
    {
        PolicyInteractionsResponse From(PolicyInteractionsResult policyInteractionsResult);
    }
    public class PolicyInteractionsResultConverter : IPolicyInteractionsResultConverter
    {
        public PolicyInteractionsResponse From(PolicyInteractionsResult policyInteractionsResult)
        {
            return new PolicyInteractionsResponse
            {
                PolicyInteractionDetailsList = policyInteractionsResult.Interactions.Select(From)
            };
        }

        private PolicyInteractionDetails From(PolicyInteractions policyInteractions)
        {
            return new PolicyInteractionDetails
            {
                DateCreated = policyInteractions.DateCreated.ToFriendlyDateTimeString(),
                InteractionType = GetInteractionTypeString(policyInteractions.InteractionType),
                CreatedBy = policyInteractions.CreatedBy
            };
        }

        private string GetInteractionTypeString(InteractionType interactionType)
        {
            return interactionType.ToFriendlyString();
        }
    }
}
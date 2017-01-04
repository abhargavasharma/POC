using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPolicyInteractionsRequestConverter
    {
        PolicyInteractionsRequest From(string quoteReferenceNumber);
    }
    public class PolicyInteractionsRequestConverter : IPolicyInteractionsRequestConverter
    {
        public PolicyInteractionsRequest From(string quoteReferenceNumber)
        {
            return PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(quoteReferenceNumber);
        }
    }
}
using TAL.QuoteAndApply.ServiceLayer.Policy;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPolicyNotesRequestConverter
    {
        PolicyNotesRequest From(string quoteReferenceNumber);
    }
    public class PolicyNotesRequestConverter : IPolicyNotesRequestConverter
    {
        public PolicyNotesRequest From(string quoteReferenceNumber)
        {
            return PolicyNotesRequest.PolicyNotesByQuoteReference(quoteReferenceNumber);
        }
    }
}
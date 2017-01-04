namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyInteractionsRequest
    {
        public string QuoteReferenceNumber { get; private set; }

        public static PolicyInteractionsRequest PolicyInteractionsByQuoteReference (string quoteReferenceNumber)
        {
            return new PolicyInteractionsRequest()
            {
                QuoteReferenceNumber = quoteReferenceNumber
            };
        }
    }
}

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public class PolicyNotesRequest
    {
        public string QuoteReferenceNumber { get; private set; }

        public static PolicyNotesRequest PolicyNotesByQuoteReference(string quoteReferenceNumber)
        {
            return new PolicyNotesRequest()
            {
                QuoteReferenceNumber = quoteReferenceNumber
            };
        }
    }
}

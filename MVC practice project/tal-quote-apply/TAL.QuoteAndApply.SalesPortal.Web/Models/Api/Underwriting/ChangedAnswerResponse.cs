namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api.Underwriting
{
    public class ChangedAnswerResponse
    {
        public ChangedAnswerResponse(string id, ChangedAttributes changedAttributes)
        {
            Id = id;
            ChangedAttributes = changedAttributes;
        }

        public string Id { get; private set; }
        public ChangedAttributes ChangedAttributes { get; private set; }
    }
}
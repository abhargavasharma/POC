using System.Configuration;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyDocumentUrlProvider
    {
        string For(string quoteReference);
    }

    public class PolicyDocumentUrlProvider : IPolicyDocumentUrlProvider
    {
        public string For(string quoteReference)
        {
            var host = ConfigurationManager.AppSettings["SalesPortalHostName"];
            return $"https://{host}/Policy/Edit/{quoteReference}";
        }
    }
}

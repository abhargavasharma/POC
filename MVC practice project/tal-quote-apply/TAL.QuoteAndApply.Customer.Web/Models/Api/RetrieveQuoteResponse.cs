using System.Collections.Generic;
using System.Linq;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class RetrieveQuoteResponse
    {
        public bool CanRetrieve => !Errors?.Any() ?? true;

        public IEnumerable<string> Errors { get; }

        public RetrieveQuoteResponse()
        {
        }

        public RetrieveQuoteResponse(params string[] errors)
        {
            Errors = errors;
        }
    }
}
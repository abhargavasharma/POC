using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models
{
    public class ExternalCustomerReferenceDetails
    {
        public ExternalCustomerRefRequired ExternalCustomerRefRequired { get; set; }
        public string ExternalCustomerRefLabel { get; set; }
        public ExternalCustomerReferenceDetails(ExternalCustomerRefRequired externalCustomerRefRequired, string externalCustomerRefLabel)
        {
            ExternalCustomerRefRequired = externalCustomerRefRequired;
            ExternalCustomerRefLabel = externalCustomerRefLabel;
        }
    }
}

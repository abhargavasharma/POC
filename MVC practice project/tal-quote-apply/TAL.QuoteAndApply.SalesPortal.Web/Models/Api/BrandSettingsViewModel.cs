using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class BrandSettingsViewModel
    {
        public ExternalCustomerRefRequired ExternalCustomerRefEnum { get; set; }
        public bool ExternalCustomerRefRequired { get; set; }
        public string ExternalCustomerRefLabel { get; set; }
    }
}
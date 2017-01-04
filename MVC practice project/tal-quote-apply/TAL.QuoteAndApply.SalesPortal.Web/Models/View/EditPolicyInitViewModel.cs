using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.View
{
    public class EditPolicyInitViewModel
    {
        public QuoteEditSource QuoteEditSource { get; set; }
        public string QuoteReferenceNumber { get; set; }
        public BrandSettingsViewModel BrandSettingsViewModel { get; set; }
    }
}
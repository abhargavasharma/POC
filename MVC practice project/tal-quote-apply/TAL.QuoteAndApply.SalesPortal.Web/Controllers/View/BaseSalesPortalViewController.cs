using System.Web.Mvc;
using TAL.QuoteAndApply.SalesPortal.Web.Services;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.View
{
    public class BaseSalesPortalViewController : Controller
    {
        private readonly ISalesPortalUiBrandingHelper _brandingHelper;

        protected BaseSalesPortalViewController(ISalesPortalUiBrandingHelper brandingHelper)
        {
            _brandingHelper = brandingHelper;
            brandingHelper.SetLoggedInBrandUi(ViewBag);
        }

        protected void SetUiBrandForQuote(string quoteReference)
        {
            _brandingHelper.SetQuoteBrandUi(quoteReference, ViewBag);
        }
    }
}
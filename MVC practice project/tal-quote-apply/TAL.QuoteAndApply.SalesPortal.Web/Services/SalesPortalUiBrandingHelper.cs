using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Policy;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public interface ISalesPortalUiBrandingHelper
    {
        void SetLoggedInBrandUi(dynamic viewBag);
        void SetQuoteBrandUi(string quoteReference, dynamic viewBag);
    }

    public class SalesPortalUiBrandingHelper : ISalesPortalUiBrandingHelper
    {
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly ISalesPortalSessionContext _salesPortalSessionContext;

        public SalesPortalUiBrandingHelper(IPolicyOverviewProvider policyOverviewProvider, ISalesPortalSessionContext salesPortalSessionContext)
        {
            _policyOverviewProvider = policyOverviewProvider;
            _salesPortalSessionContext = salesPortalSessionContext;
        }

        public void SetLoggedInBrandUi(dynamic viewBag)
        {
            const string defaultBrandCode = "TAL"; //TODO: grab default brand from BrandSettings
            SetUiBrand(_salesPortalSessionContext.SalesPortalSession?.SelectedBrand ?? defaultBrandCode, viewBag);
        }

        public void SetQuoteBrandUi(string quoteReference, dynamic viewBag)
        {
            var policyOverview = _policyOverviewProvider.GetFor(quoteReference);
            SetUiBrand(policyOverview.Brand, viewBag);
        }

        private void SetUiBrand(string brandCode, dynamic viewBag)
        {
            viewBag.Brand = brandCode.ToLower();
        }

    }
}
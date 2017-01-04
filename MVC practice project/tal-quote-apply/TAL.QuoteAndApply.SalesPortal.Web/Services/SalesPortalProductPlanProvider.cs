using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public class SalesPortalCurrentProductBrandProvider : ICurrentProductBrandProvider
    {
        private readonly ISalesPortalSessionContext _salesPortalSessionContext;
        private readonly IProductBrandProvider _brandProvider;
        private readonly IOrganisationProvider _organisationProvider;

        public SalesPortalCurrentProductBrandProvider(ISalesPortalSessionContext salesPortalSessionContext,
            IProductBrandProvider brandProvider, IOrganisationProvider organisationProvider)
        {
            _salesPortalSessionContext = salesPortalSessionContext;
            _brandProvider = brandProvider;
            _organisationProvider = organisationProvider;
        }

        public ProductBrand GetCurrent()
        {
            var selectedBrand = "TAL";
            if (_salesPortalSessionContext?.SalesPortalSession?.SelectedBrand != null)
            {
                selectedBrand = _salesPortalSessionContext.SalesPortalSession.SelectedBrand;
            }
            var selectedBrandId = _brandProvider.GetBrandIdByKey(selectedBrand);
            return new ProductBrand(selectedBrandId, selectedBrand, _organisationProvider.GetDefaultOrganisationIdByBrandId(selectedBrandId));
        }
    }
}
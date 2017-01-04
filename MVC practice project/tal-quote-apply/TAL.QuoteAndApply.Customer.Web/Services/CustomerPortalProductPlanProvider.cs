using System;
using TAL.QuoteAndApply.Customer.Web.Configuration;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public class CustomerPortalCurrentProductBrandProvider : ICurrentProductBrandProvider
    {
        private readonly IProductBrandProvider _productBrandProvider;
        private readonly IOrganisationProvider _organisationProvider;

        public CustomerPortalCurrentProductBrandProvider(IProductBrandProvider productBrandProvider, 
            IOrganisationProvider organisationProvider)
        {
            _productBrandProvider = productBrandProvider;
            _organisationProvider = organisationProvider;
        }

        public ProductBrand GetCurrent()
        {
            string brandKey = new BrandSettingsProvider().BrandKey;
            if (_productBrandProvider.CheckIfBrandExists(brandKey))
            {
                var brandId = _productBrandProvider.GetBrandIdByKey(brandKey);
                var organisationId = _organisationProvider.GetDefaultOrganisationIdByBrandId(brandId);
                return new ProductBrand(brandId, brandKey, organisationId);
            }
            
            throw new ApplicationException($"Could not find brand '{brandKey}'. Please make sure it exists in the Database");
        }
    }
}
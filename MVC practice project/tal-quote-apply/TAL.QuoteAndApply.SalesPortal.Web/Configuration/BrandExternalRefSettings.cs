using System;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;

namespace TAL.QuoteAndApply.SalesPortal.Web.Configuration
{
    public interface IBrandExternalRefSettings
    {
        BrandSettingsViewModel ExternalCustomerRefSettings();
    }
    public class BrandExternalRefSettings : IBrandExternalRefSettings
    {
        private readonly ISalesPortalSessionContext _salesPortalSessionContext;
        private readonly IExternalRefDetailsFactory _externalRefDetailsFactory;
        private readonly IExternalCustomerReferenceViewModelConverter _externalCustomerReferenceViewModelConverter;
        public BrandExternalRefSettings( ISalesPortalSessionContext salesPortalSessionContext,
           IExternalRefDetailsFactory externalRefDetailsFactory,
           IExternalCustomerReferenceViewModelConverter externalCustomerReferenceViewModelConverter)
        {
            _salesPortalSessionContext = salesPortalSessionContext;
            _externalRefDetailsFactory = externalRefDetailsFactory;
            _externalCustomerReferenceViewModelConverter = externalCustomerReferenceViewModelConverter;
        }

        public BrandSettingsViewModel ExternalCustomerRefSettings()
        {
            string brandKey = _salesPortalSessionContext.SalesPortalSession.SelectedBrand;
            if (!string.IsNullOrEmpty(brandKey))
            {
                var externalRef = _externalRefDetailsFactory.ExternalCustomerRefConfigDetails(brandKey);
                var externalRefDetails = _externalCustomerReferenceViewModelConverter.From(externalRef);
                return externalRefDetails;
            }
            throw new ApplicationException("There is no Brand associated with the session");
        }
    }
}
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IExternalCustomerReferenceViewModelConverter
    {
        BrandSettingsViewModel From(ExternalCustomerReferenceDetails externalCustomerReferenceDetails);
    }

    public class ExternalCustomerReferenceViewModelConverter : IExternalCustomerReferenceViewModelConverter
    {
        public BrandSettingsViewModel From(ExternalCustomerReferenceDetails externalCustomerReferenceDetails)
        {
            var externalRefRequired =false;
            if (externalCustomerReferenceDetails.ExternalCustomerRefRequired == ExternalCustomerRefRequired.Mandatory ||
                externalCustomerReferenceDetails.ExternalCustomerRefRequired == ExternalCustomerRefRequired.Optional)
                    externalRefRequired = true;
            else
            {
                externalRefRequired = false;
            }
            return new BrandSettingsViewModel
            {
                ExternalCustomerRefRequired = externalRefRequired,
                ExternalCustomerRefLabel = externalCustomerReferenceDetails.ExternalCustomerRefLabel,
                ExternalCustomerRefEnum = externalCustomerReferenceDetails.ExternalCustomerRefRequired
            };
        }
    }
}
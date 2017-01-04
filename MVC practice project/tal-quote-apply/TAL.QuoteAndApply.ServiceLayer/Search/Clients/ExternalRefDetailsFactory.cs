using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients
{
    public interface IExternalRefDetailsFactory
    {
        ExternalCustomerReferenceDetails ExternalCustomerRefConfigDetails(string brandKey);
    }

    public class ExternalRefDetailsFactory : IExternalRefDetailsFactory
    {
        private readonly IProductDefinitionProvider _productDefinitionProvider;

        public ExternalRefDetailsFactory(IProductDefinitionProvider productDefinitionProvider)
        {
            _productDefinitionProvider = productDefinitionProvider;
        }

        public ExternalCustomerReferenceDetails ExternalCustomerRefConfigDetails(string brandKey)
        {
            var externalCustomerRefDetails = _productDefinitionProvider.GetExternalCustomerRefConfigDetails(brandKey);
            return new ExternalCustomerReferenceDetails(externalCustomerRefDetails.ExternalCustomerRefRequired,externalCustomerRefDetails.ExternalCustomerRefLabel );
        }
    }
}

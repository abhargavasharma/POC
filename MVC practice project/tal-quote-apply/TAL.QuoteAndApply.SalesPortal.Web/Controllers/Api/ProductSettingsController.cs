using System.Linq;
using System.Web.Http;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/settings/{quoteReferenceNumber}")]
    public class ProductSettingsController: ApiController
    {
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly IProductBrandProvider _productBrandProvider;

        public ProductSettingsController(IProductDefinitionProvider productDefinitionProvider, IProductBrandProvider productBrandProvider)
        {
            _productDefinitionProvider = productDefinitionProvider;
            _productBrandProvider = productBrandProvider;
        }

        [HttpGet, Route("ownerTypes")]
        public IHttpActionResult GetAvailablePolicyOwnerTypes(string quoteReferenceNumber)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
            var productDefinition = _productDefinitionProvider.GetProductDefinition(brandKey);

            return Ok(productDefinition.AvailableOwnerTypes);
        }

        [HttpGet, Route("paymentOptions")]
        public IHttpActionResult GetPaymentOptionsForProduct(string quoteReferenceNumber)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
            var productDefinition = _productDefinitionProvider.GetProductDefinition(brandKey);

            return Ok(new
            {
                IsDirectDebitAvailable = productDefinition.IsDirectDebitAvailable,
                IsCreditCardAvailable = productDefinition.AvailableCreditCardTypes != null && productDefinition.AvailableCreditCardTypes.Any(),
                IsSuperAvailable = productDefinition.IsSuperannuationAvailable,
                IsSmsfAvailable = productDefinition.IsSmsfAvailable
            });
        }

        [HttpGet, Route("creditCards")]
        public IHttpActionResult GetAvailableCreditCardTypes(string quoteReferenceNumber)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
            var productDefinition = _productDefinitionProvider.GetProductDefinition(brandKey);

            return Ok(productDefinition.AvailableCreditCardTypes);
        }

        [HttpGet, Route("maxBeneficiaries")]
        public IHttpActionResult GetMaximumNumberOfBeneficiaries(string quoteReferenceNumber)
        {
            var brandKey = _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
            var maxBeneficiaries = _productDefinitionProvider.GetProductDefinition(brandKey).MaximumNumberOfBeneficiaries;
            return Ok(maxBeneficiaries);
        }
    }
}
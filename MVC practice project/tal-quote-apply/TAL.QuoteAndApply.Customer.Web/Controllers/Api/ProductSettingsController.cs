using System.Linq;
using System.Web.Http;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [WebApiQuoteSessionRequired]
    [RoutePrefix("api/settings")]
    public class ProductSettingsController: BaseCustomerPortalApiController
    {
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;

        public ProductSettingsController(IQuoteSessionContext quoteSessionContext, 
            IPolicyOverviewProvider policyOverviewProvider, 
            ICurrentProductBrandProvider currentProductBrandProvider, 
            IProductDefinitionProvider productDefinitionProvider) : 
            base(quoteSessionContext, policyOverviewProvider)
        {
            _currentProductBrandProvider = currentProductBrandProvider;
            _productDefinitionProvider = productDefinitionProvider;
        }
        
        [HttpGet, Route("maxBeneficiaries")]
        public IHttpActionResult GetMaximumNumberOfBeneficiaries()
        {
            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var productDefinition = _productDefinitionProvider.GetProductDefinition(currentBrand.BrandCode);
            
            return Ok(productDefinition.MaximumNumberOfBeneficiaries);
        }

        [HttpGet, Route("paymentOptions")]
        public IHttpActionResult GetPaymentOptionsForProduct()
        {
            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var productDefinition = _productDefinitionProvider.GetProductDefinition(currentBrand.BrandCode);

            var result = new AvailablePaymentOptionsViewModel
            {
                IsDirectDebitAvailable = productDefinition.IsDirectDebitAvailable,
                IsCreditCardAvailable = productDefinition.AvailableCreditCardTypes != null && productDefinition.AvailableCreditCardTypes.Any(),
                IsSuperAvailable = productDefinition.IsSuperannuationAvailable
            };

            return Ok(result);
        }

        [HttpGet, Route("creditCardTypes")]
        public IHttpActionResult GetAvailableCreditCardTypes()
        {
            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var productDefinition = _productDefinitionProvider.GetProductDefinition(currentBrand.BrandCode);

            return Ok(productDefinition.AvailableCreditCardTypes);
        }

        [HttpGet, Route("isQuoteRetrivalAvailable")]
        public IHttpActionResult IsQuoteRetrivalAvailable()
        {
            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var productDefinition = _productDefinitionProvider.GetProductDefinition(currentBrand.BrandCode);

            return Ok(productDefinition.IsQuoteSaveLoadEnabled);
        }

    }
}
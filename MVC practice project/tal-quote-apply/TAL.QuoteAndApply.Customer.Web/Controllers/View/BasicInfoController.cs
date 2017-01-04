using System.Web.Mvc;
using TAL.QuoteAndApply.Customer.Web.Configuration;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.View
{
    public class BasicInfoController : Controller
    {
        private readonly ICaptchaConfigurationProvider _captchaConfig;
        private readonly ICaclculatorConfigurationProvider _calculatorConfig;
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;

        public BasicInfoController(ICaptchaConfigurationProvider captchaConfig, ICaclculatorConfigurationProvider calculatorConfig, ICurrentProductBrandProvider currentProductBrandProvider, IProductDefinitionProvider productDefinitionProvider)
        {
            _captchaConfig = captchaConfig;
            _calculatorConfig = calculatorConfig;
            _currentProductBrandProvider = currentProductBrandProvider;
            _productDefinitionProvider = productDefinitionProvider;
        }

        // GET: BasicInfo
        public ActionResult Index()
        {
            ViewBag.IsCalculatorEnabled = _calculatorConfig.IsEnabled;
            ViewBag.CoverCalculatorScriptsUrl = _calculatorConfig.CoverCalculatorScriptsUrl;
            ViewBag.CoverCalculatorBaseUrl = _calculatorConfig.CoverCalculatorBaseUrl;
            ViewBag.UseCaptcha = _captchaConfig.IsEnabled;
            ViewBag.JourneyType = PolicySource.CustomerPortalBuildMyOwn;

            var currentBrand = _currentProductBrandProvider.GetCurrent();
            var productDefinition = _productDefinitionProvider.GetProductDefinition(currentBrand.BrandCode);
            ViewBag.IsQuoteSaveLoadEnabled = productDefinition.IsQuoteSaveLoadEnabled;

            return View();
        }
    }
}
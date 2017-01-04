using System.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Configuration;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.View
{

    public class SelectCoverController : BaseCustomerViewController
    {
        private readonly IPolicyInitialisationMetadataService _policyInitialisationMetadataService;
        private readonly ICaclculatorConfigurationProvider _calculatorConfig;


        public SelectCoverController(IBaseCustomerControllerHelper baseCustomerControllerHelper,
            IPolicyInitialisationMetadataService policyInitialisationMetadataService,
            ICaclculatorConfigurationProvider calculatorConfig) : base(baseCustomerControllerHelper)
        {
            _policyInitialisationMetadataService = policyInitialisationMetadataService;
            _calculatorConfig = calculatorConfig;
        }

        // GET: SelectCover
        public ActionResult Index()
        {
            ViewBag.IsCalculatorEnabled = _calculatorConfig.IsEnabled;
            ViewBag.CoverCalculatorScriptsUrl = _calculatorConfig.CoverCalculatorScriptsUrl;
            ViewBag.CoverCalculatorBaseUrl = _calculatorConfig.CoverCalculatorBaseUrl;

            var policyInitialisationMetaData = _policyInitialisationMetadataService.GetPolicyInitialisationMetadataForSession();
            ViewBag.initMetaData = JsonConvert.SerializeObject(policyInitialisationMetaData);

            return View();
        }

    }
}
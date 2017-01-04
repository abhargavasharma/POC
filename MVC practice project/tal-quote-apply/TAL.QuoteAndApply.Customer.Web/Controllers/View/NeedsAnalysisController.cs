using System.Web.Mvc;
using TAL.QuoteAndApply.Customer.Web.Configuration;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.View
{
    public class NeedsAnalysisController : Controller
    {
        private readonly IQuoteSessionContext _quoteSessionContext;
		private readonly ICaptchaConfigurationProvider _captchaConfig;
        private readonly IAngularConfigurationProvider _angularConfigurationProvider;

        public NeedsAnalysisController(IQuoteSessionContext quoteSessionContext,
            ICaptchaConfigurationProvider captchaConfig, IAngularConfigurationProvider angularConfigurationProvider)
        {
            _quoteSessionContext = quoteSessionContext;
            _captchaConfig = captchaConfig;
            _angularConfigurationProvider = angularConfigurationProvider;
        }

        // GET: NeedsAnalysis
        public ActionResult Index()
        {
            ViewBag.JourneyType = PolicySource.CustomerPortalHelpMeChoose;
            ViewBag.UseCaptcha = _captchaConfig.IsEnabled;
            ViewBag.DebugEnabled = _angularConfigurationProvider.DebugEnabled;
			ViewBag.QuoteReference = _quoteSessionContext.QuoteSession?.QuoteReference ?? string.Empty;
            return View();
        }
    }
}
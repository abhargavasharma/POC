using System.Web.Mvc;
using TAL.QuoteAndApply.Analytics.Models;
using TAL.QuoteAndApply.Analytics.Service;
using TAL.QuoteAndApply.Customer.Web.Models.View;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.View
{
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsSiteHeaderBuilder _analyticsSiteHeaderBuilder;

        public AnalyticsController(IAnalyticsSiteHeaderBuilder analyticsSiteHeaderBuilder)
        {
            _analyticsSiteHeaderBuilder = analyticsSiteHeaderBuilder;
        }

        // GET: Analytics
        public ActionResult Header(AnalyticsPageModel model)
        {
            var content = _analyticsSiteHeaderBuilder.BuildHeader(model);
            return View("Header", new AnalyticsViewModel { HtmlData = content});
        }

        public ActionResult Footer()
        {
            var content = _analyticsSiteHeaderBuilder.BuildFooter();
            return View("Footer", new AnalyticsViewModel { HtmlData = content });
        }
    }
}
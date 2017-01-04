using System.Web.Mvc;
using TAL.QuoteAndApply.Customer.Web.Models.View;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.View
{
    public class RetrieveController : Controller
    {
        public ActionResult Index(string id)
        {
            var retrieveQuoteRequest = new RetrieveQuoteRequest()
            {
                QuoteReference = id
            };
            return View(retrieveQuoteRequest);
        }
    }
}
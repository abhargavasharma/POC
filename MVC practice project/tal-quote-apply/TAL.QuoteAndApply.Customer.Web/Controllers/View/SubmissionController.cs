using System.Web.Mvc;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.View
{
    public class SubmissionController : BaseCustomerViewController
    {

        public ActionResult Index()
        {
            base.ClearSession();
            return View();
        }

        public SubmissionController(IBaseCustomerControllerHelper baseCustomerControllerHelper) : base(baseCustomerControllerHelper)
        {
        }
    }
}
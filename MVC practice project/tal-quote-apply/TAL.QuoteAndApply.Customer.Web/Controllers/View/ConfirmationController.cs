using System.Web.Mvc;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.View
{
    public class ConfirmationController : BaseCustomerViewController
    {
        
        public ActionResult Index()
        {
            base.ClearSession();
            return View();
        }

        public ConfirmationController(IBaseCustomerControllerHelper baseCustomerControllerHelper)
            : base(baseCustomerControllerHelper)
        {
        }
    }
}
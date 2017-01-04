using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.View
{
    public class ErrorController : Controller
    {
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            
            return View();
        }

        public ActionResult Index()
        {
            Response.StatusCode = 500;

            return View("Error");
        }

        public ActionResult Generate()
        {
            throw new ApplicationException();
        }

    }
}
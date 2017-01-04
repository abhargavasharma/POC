using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TAL.QuoteAndApply.Customer.Web
{
    public class GlobalViewBagFilter : ActionFilterAttribute
    {
        public string AnalyticsBaseUrl => ConfigurationManager.AppSettings["Analytics.BaseUrl"];

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.AnalyticsBaseUrl = AnalyticsBaseUrl;
        }
    }
}
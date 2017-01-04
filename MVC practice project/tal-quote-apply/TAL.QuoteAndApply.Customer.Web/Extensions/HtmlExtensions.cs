using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TAL.QuoteAndApply.Customer.Web.Configuration;

namespace TAL.QuoteAndApply.Customer.Web.Extensions
{
    public static class CustomerHtmlHelper
    {

        public static string CustomBrandStyle(this HtmlHelper helper)
        {
            var brandSettings = DependencyResolver.Current.GetService<IBrandSettingsProvider>();
            return brandSettings.BrandCssClass ?? "";
        }

        public static string CustomBrandPath(this HtmlHelper helper)
        {
            var brandSettings = DependencyResolver.Current.GetService<IBrandSettingsProvider>();
            return brandSettings.BrandKey ?? "";
        }
    }
}
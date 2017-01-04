using System.Web.Mvc;
using TAL.QuoteAndApply.Customer.Web.Attributes;

namespace TAL.QuoteAndApply.Customer.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new NoCacheAttribute());
        }
    }
}
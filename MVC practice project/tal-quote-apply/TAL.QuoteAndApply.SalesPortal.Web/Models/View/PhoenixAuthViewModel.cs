using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.View
{
    public class PhoenixAuthViewModel
    {
        public string PhoenixUrl { get; set; }
        public string AuthenticationToken { get; set; }
        public string CallBackOnErrorUrl { get; set; }
        public string CallBackOnSuccessUrl { get; set; }
    }
}
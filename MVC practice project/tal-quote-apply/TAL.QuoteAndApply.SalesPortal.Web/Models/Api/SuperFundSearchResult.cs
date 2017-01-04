using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class SuperFundSearchResult
    {
        public string FundUSI { get; set; }
        public string FundName { get; set; }
        public string FundABN { get; set; }
        public string FundProduct { get; set; }
    }
}
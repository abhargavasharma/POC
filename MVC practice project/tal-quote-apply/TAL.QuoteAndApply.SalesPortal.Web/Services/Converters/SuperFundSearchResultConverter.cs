using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface ISuperFundSearchResultConverter
    {
        SuperFundSearchResult From(SuperannuationFundSearchResponse superannuationFundSearch);
    }

    public class SuperFundSearchResultConverter : ISuperFundSearchResultConverter
    {
        public SuperFundSearchResult From(SuperannuationFundSearchResponse superannuationFundSearch)
        {
            return new SuperFundSearchResult()
            {
                FundABN = superannuationFundSearch.FundABN,
                FundUSI = superannuationFundSearch.FundUSI,
                FundName = superannuationFundSearch.FundName,
                FundProduct = superannuationFundSearch.FundProduct
            };
        }
    }
}
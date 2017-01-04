using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Payment;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/superannuation")]
    public class SuperannuationController : ApiController
    {
        private readonly ISuperFundSearchResultConverter _superFundSearchResultConverter;
        private readonly ISuperannuationFundSearch _superannuationFundSearch;

        public SuperannuationController(ISuperFundSearchResultConverter superFundSearchResultConverter,
            ISuperannuationFundSearch superannuationFundSearch)
        {
            _superFundSearchResultConverter = superFundSearchResultConverter;
            _superannuationFundSearch = superannuationFundSearch;
        }

        [HttpGet, Route("search/abn/{abn}")]
        public IHttpActionResult SearchSuperannuationByAbn(string abn)
        {
            var results = _superannuationFundSearch.GetSuperFundsByABN(abn);
            var retVal = results.Select(_superFundSearchResultConverter.From);
            return Ok(retVal);
        }

        [HttpGet, Route("search/abn/{abn}/{product}")]
        public IHttpActionResult SearchSuperannuationByAbnAndProduct(string abn, string product)
        {
            var results = _superannuationFundSearch.GetSuperFundsByABNAndProduct(abn, product);
            var retVal = results.Select(_superFundSearchResultConverter.From);
            return Ok(retVal);
        }

        [HttpGet, Route("search/organisation/{fundName}")]
        public IHttpActionResult SearchSuperannuationByFundName(string fundName)
        {
            var results = _superannuationFundSearch.GetSuperFundsByName(fundName);
            var retVal = results.Select(_superFundSearchResultConverter.From);
            return Ok(retVal);
        }

        [HttpGet, Route("search/organisation/{fundName}/{product}")]
        public IHttpActionResult SearchSuperannuationByFundNameAndProduct(string fundName, string product)
        {
            var results = _superannuationFundSearch.GetSuperFundsByNameAndProduct(fundName, product);
            var retVal = results.Select(_superFundSearchResultConverter.From);
            return Ok(retVal);
        }
    }
}

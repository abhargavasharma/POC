using System.Web.Http;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using IPolicyInteractionsResultConverter = TAL.QuoteAndApply.SalesPortal.Web.Services.Converters.IPolicyInteractionsResultConverter;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/policy/{quoteReferenceNumber}")]
    public class InteractionsController : ApiController
    {

        private readonly IPolicyInteractionService _policyInteractionService;
        private readonly IPolicyInteractionsRequestConverter _policyInteractionsRequestConverter;
        private readonly IPolicyInteractionsResultConverter _policyInteractionsResultConverter;


        public InteractionsController  (IPolicyInteractionService policyInteractionService, 
                                        IPolicyInteractionsRequestConverter policyInteractionsRequestConverter,
                                        IPolicyInteractionsResultConverter policyInteractionsResultConverter)

        {
            _policyInteractionService = policyInteractionService;
            _policyInteractionsRequestConverter = policyInteractionsRequestConverter;
            _policyInteractionsResultConverter = policyInteractionsResultConverter;
        }

        [HttpGet, Route("interactions")]
        public IHttpActionResult GetInteractions(string quoteReferenceNumber)
        {
            var request = _policyInteractionsRequestConverter.From(quoteReferenceNumber);
            var searchResult = _policyInteractionService.GetInteractions(request);
            return Ok(_policyInteractionsResultConverter.From(searchResult));
        }
    }
}
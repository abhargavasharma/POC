using System.Web.Http;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Policy;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/policy/{quoteReferenceNumber}/risk/{riskId:int}")]
    public class CorrespondenceController : ApiController
    {

        private readonly IPolicyCorrespondenceService _policyCorrespondenceService;

        public CorrespondenceController(IPolicyCorrespondenceService policyCorrespondenceService)
        {
            _policyCorrespondenceService = policyCorrespondenceService;
        }

        [HttpGet, Route("correspondence")]
        public IHttpActionResult GetCorrespondenceSummary(string quoteReferenceNumber, int riskId)
        {
            var correspondenceSummary = _policyCorrespondenceService.GetCorrespondenceSummary(quoteReferenceNumber, riskId);
            return Ok(correspondenceSummary);
        }

        [HttpPost, Route("correspond")]
        public IHttpActionResult SendCorrespondence(string quoteReferenceNumber)
        {
            var emailSent = _policyCorrespondenceService.SendCorrespondence(quoteReferenceNumber, CorrespondenceEmailType.SaveQuote);
            return Ok(emailSent);
        }
    }
}

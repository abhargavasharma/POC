using System.Threading.Tasks;
using System.Web.Http;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [RoutePrefix("api/raise")]
    public class RaisePolicyController : ApiController
    {
        private readonly IRaisePolicyFeedbackService _raisePolicyFeedbackService;

        public RaisePolicyController(IRaisePolicyFeedbackService raisePolicyFeedbackService)
        {
            _raisePolicyFeedbackService = raisePolicyFeedbackService;
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> RaisePolicyResult()
        {
            string result = await Request.Content.ReadAsStringAsync();
            _raisePolicyFeedbackService.ProcessRaisePolicyFeedback(result);

            return Ok();
        }
    }
}

using System.Web.Http;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.ManageInterview;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [RoutePrefix("api/postsales/manageInterview/{interviewId}")]
    public class ManageInterviewController : ApiController
    {
        private readonly ITalusUiUrlService _talusUiUrlService;
        private readonly IManageInterviewService _manageInterviewService;


        public ManageInterviewController(IManageInterviewService manageInterviewService,
                                         ITalusUiUrlService talusUiUrlService)
        {
            _manageInterviewService = manageInterviewService;
            _talusUiUrlService = talusUiUrlService;
        }

        [HttpGet, Route("find")]
        public IHttpActionResult FindInterview(string interviewId)
        {
            var interviewExists = _manageInterviewService.InterviewExists(interviewId);
            
            return Ok(interviewExists);
        }
    }
}
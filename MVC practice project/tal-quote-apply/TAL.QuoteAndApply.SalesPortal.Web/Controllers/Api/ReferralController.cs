using System.Web.Http;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy.Referral;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/policy/{quoteReferenceNumber}")]
    public class ReferralController : ApiController
    {
        private readonly ICreateReferralService _createReferralService;
        private readonly ICompleteReferralService _completeReferralService;
        private readonly IAssignReferralService _assignReferralService;
        private readonly IReferralDetailsResultConverter _referralDetailsResultConverter;

        public ReferralController(ICreateReferralService createReferralService, 
            ICompleteReferralService completeReferralService,
            IAssignReferralService assignReferralService, 
            IReferralDetailsResultConverter referralDetailsResultConverter)
        {
            _createReferralService = createReferralService;
            _completeReferralService = completeReferralService;
            _assignReferralService = assignReferralService;
            _referralDetailsResultConverter = referralDetailsResultConverter;
        }

        [HttpPost, Route("referral")]
        public IHttpActionResult CreateReferral(string quoteReferenceNumber)
        {
            var result = _createReferralService.CreateReferralFor(quoteReferenceNumber);

            if (result == CreateReferralResult.Created)
            {
                return Ok();
            }

            ModelState.AddModelError("referral", "An in progress referral already exists for this policy");
            return new InvalidModelStateActionResult(ModelState);
        }
        
        [HttpPost, Route("referral/complete")]
        public IHttpActionResult CompleteReferral(string quoteReferenceNumber)
        {
            _completeReferralService.CompleteReferral(quoteReferenceNumber);
            return Ok();
        }

        [RoleFilter(Role.Underwriter)]
        [HttpPost, Route("assign")]
        public IHttpActionResult AssignReferralToUnderwriter(string quoteReferenceNumber, AssignReferralRequest assignReferralRequest)
        {
            var assignedReferral = _referralDetailsResultConverter.From(_assignReferralService.Assign(quoteReferenceNumber, assignReferralRequest.Name));
            return Ok(assignedReferral);
        }
    }
}

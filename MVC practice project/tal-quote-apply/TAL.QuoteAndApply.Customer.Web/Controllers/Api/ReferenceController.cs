using System.Web.Http;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [RoutePrefix("api/reference")]
    public class ReferenceController : ApiController
    {
        private readonly IBeneficiaryDetailsService _beneficiaryDetailsService;

        public ReferenceController(IBeneficiaryDetailsService beneficiaryDetailsService)
        {
            _beneficiaryDetailsService = beneficiaryDetailsService;
        }

        [HttpGet, Route("beneficiaryRelationships")]
        public IHttpActionResult GetRelationshipsToTheInsured()
        {
            var relationshipsToTheInsured = _beneficiaryDetailsService.GetBeneficiaryRelationships();
            return Ok(relationshipsToTheInsured);
        }
    }
}

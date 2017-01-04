using System.Linq;
using System.Web.Http;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    public class AliveController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok("Alive :)");
        }
    }
}

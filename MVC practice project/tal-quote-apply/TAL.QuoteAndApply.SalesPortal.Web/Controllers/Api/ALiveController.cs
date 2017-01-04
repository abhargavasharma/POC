
using System.Web.Http;
using TAL.QuoteAndApply.Infrastructure.Logging;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    public class AliveController : ApiController
    {
        private readonly ILoggingService _log;

        public AliveController(ILoggingService log)
        {
            _log = log;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            _log.Info("Alive service hit and logging");
            return Ok(new {Message = "I'm alive :)"});
        }
    }
}
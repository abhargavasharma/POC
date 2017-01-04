using System.Web.Mvc;
using TAL.QuoteAndApply.Customer.Web.Attributes;
using TAL.QuoteAndApply.Customer.Web.Converters;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.ServiceLayer.Policy;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.View
{
    public class EntryController : Controller
    {
        private readonly IPolicyInitialisationMetadataService _policyInitialisationMetadataService;
        private readonly IPolicyInitialisationMetadataProvider _policyInitialisationMetadataProvider;
        private readonly ILoggingService _loggingService;

        public EntryController(IPolicyInitialisationMetadataService policyInitialisationMetadataService, IPolicyInitialisationMetadataProvider policyInitialisationMetadataProvider, ILoggingService loggingService)
        {
            _policyInitialisationMetadataService = policyInitialisationMetadataService;
            _policyInitialisationMetadataProvider = policyInitialisationMetadataProvider;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Entry point to the application from tal.com.au
        /// </summary>
        /// <param name="talQuoteAndApplyEntryPayload">The name of the form field posted from tal.com.au. Important that this is not renamed</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index([JsonDeserialize]EntryPointViewModel talQuoteAndApplyEntryPayload)
        {
            if (!ModelState.IsValid || talQuoteAndApplyEntryPayload?.CalculatorResultsJson == null || talQuoteAndApplyEntryPayload?.CalculatorAssumptionsJson == null)
            {
                _loggingService.Error("Entry payload failed validation");

                //well too bad, got straight to basic info without dropping the cookie
                return RedirectToAction("Index", "BasicInfo");
            }

            var policyInitialisationMetadata = _policyInitialisationMetadataProvider.GetPolicyInitialisationMetadata(talQuoteAndApplyEntryPayload);
            _policyInitialisationMetadataService.SetPolicyInitialisationMetadataForSession(policyInitialisationMetadata);

            return RedirectToAction("Index", "BasicInfo");
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.Web.Shared.Workflow;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [Route("api/retrieve")]
    public class RetrieveController : ApiController
    {
        private readonly ICustomerPolicyRetrievalService _policyRetrievalService;
        private readonly IApplicationStepContextService _applicationStepContextService;
        private readonly IApplicationStepWorkFlowProvider _applicationStepWorkFlowProvider;

        public RetrieveController(ICustomerPolicyRetrievalService policyRetrievalService, 
                                  IApplicationStepContextService applicationStepContextService,
                                  IApplicationStepWorkFlowProvider applicationStepWorkFlowProvider)
        {
            _policyRetrievalService = policyRetrievalService;
            _applicationStepContextService = applicationStepContextService;
            _applicationStepWorkFlowProvider = applicationStepWorkFlowProvider;
        }
        
        public IHttpActionResult RetrieveQuote(RetrieveQuoteRequest retrieveQuoteRequest)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var policyRetrievalResult = _policyRetrievalService.RetrieveQuote(retrieveQuoteRequest.QuoteReference,
                retrieveQuoteRequest.Password);

            if (!policyRetrievalResult.CanRetrieve)
            {
                ModelState.AddModelError("retrieval", policyRetrievalResult.Errors.First());
                return new InvalidModelStateActionResult(ModelState);
            }

            var applicationContext = _applicationStepContextService.Get();
            var workFlow = _applicationStepWorkFlowProvider.GetForProduct(ProductCodeConstants.ProductCode);
            IEnumerable<ApplicationStep> listOfValidSteps = workFlow.ResolveAllValidSteps(applicationContext);
            var reviewStep = listOfValidSteps.Any(step => step.CurrentStepUri.Path.Contains("Review"));
            var redirectUrl = Url.Route("Default", reviewStep ? new { Controller = "Review", Action = "Index" } : new { Controller = "SelectCover", Action = "Index" });

            return new RedirectActionResult(redirectUrl);
        }
    }
}
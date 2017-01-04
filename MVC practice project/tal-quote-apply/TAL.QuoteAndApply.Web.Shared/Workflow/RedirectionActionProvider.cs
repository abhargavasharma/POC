using System;
using System.Linq;
using System.Monads;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public class RedirectionActionResult
    {
        public bool ShouldIRedirect { get; set; }
        public string RedirectUrl { get; set; }

        public static RedirectionActionResult Stay()
        {
            return new RedirectionActionResult()
            {
                ShouldIRedirect = false
            };
        }

        public static RedirectionActionResult Leave(string toUri)
        {
            if (string.IsNullOrWhiteSpace(toUri))
            {
                return Stay();
            }

            return new RedirectionActionResult()
            {
                ShouldIRedirect = true,
                RedirectUrl = toUri
            };
        }
    }

    public interface IRedirectionActionProvider
    {
        RedirectionActionResult Get(Uri currentUri);
    }

    public class RedirectionActionProvider : IRedirectionActionProvider
    {
        private readonly IApplicationStepContextService _stepContextService;
        private readonly IApplicationStepWorkFlowProvider _applicationStepWorkFlowProvider;

        public RedirectionActionProvider(IApplicationStepContextService stepContextService,
            IApplicationStepWorkFlowProvider applicationStepWorkFlowProvider)
        {
            _stepContextService = stepContextService;
            _applicationStepWorkFlowProvider = applicationStepWorkFlowProvider;
        }
        
        public RedirectionActionResult Get(Uri currentUri)
        {
            var stepContext = _stepContextService.Get();

            var workFlow = _applicationStepWorkFlowProvider.GetForProduct(ProductCodeConstants.ProductCode);
            if (stepContext.Application == null)
            {
                // the application could not be resolved. Insurance application context does not exist
                var pagePathWithoutQueryString = currentUri.GetLeftPart(UriPartial.Path);

                var firstStepUri = workFlow.GetFirstStep()
                    .With(ctx => ctx.ApplicationStep)
                    .With(step => step.CurrentStepUri)
                    .With(uri => uri.ToString()) ?? "quote";

                if (pagePathWithoutQueryString.EndsWith("/" + firstStepUri, StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectionActionResult.Stay();
                }

                return RedirectionActionResult.Leave(firstStepUri);
            }

            var steps = workFlow.ResolveAllValidSteps(stepContext);
            var canStayHere = steps.Where(step => step.IsRequestValidForStep(stepContext)); // Refactor this to use step

            if (canStayHere.Any(step => (step == steps.Last()) || step.CanArriveToThisStepBackwards(stepContext)))
                return RedirectionActionResult.Stay();
            var stepToGoTo = steps.LastOrDefault().With(step => step.CurrentStepUri.ToString());
            return RedirectionActionResult.Leave(stepToGoTo);
        }
    }
}
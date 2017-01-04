using System;
using System.Monads;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public class ApplicationContext
    {
        private readonly IApplicationStepContextService _applicationStepContextService;
        private readonly Lazy<ApplicationStepContext> _currentStepContext;

        public ApplicationContext(IApplicationStepContextService applicationStepContextService, PolicyOverviewResult application, StepWorkflowContext stepWorkflowItem)
        {
            Application = application;
            StepWorkflowItem = stepWorkflowItem;
            _applicationStepContextService = applicationStepContextService;
            _currentStepContext = new Lazy<ApplicationStepContext>(() => _applicationStepContextService.With(p => p.Get()));
        }

        public readonly PolicyOverviewResult Application;
        public readonly StepWorkflowContext StepWorkflowItem;
        
        public string ApplicationId
        {
            get
            {
                return Application.With(app => app.QuoteReferenceNumber);
            }
        }

        public string NextPageOrDefault(string defaultRelativeUri = "")
        {
            var stepContext = _currentStepContext.Value;
            return StepWorkflowItem.With(step => step.ApplicationStep)
                .With(step => step.GetNextStep(stepContext))
                .With(nextStep => nextStep.CurrentStepUri)
                .Return(uri => uri.ToString(), defaultRelativeUri);
        }

        public string PreviousPageOrDefault(string defaultRelativeUri = "")
        {
            var stepContext = _currentStepContext.Value;
            return StepWorkflowItem.With(step => step.ApplicationStep)
                .With(step => step.GetPreviousStep(stepContext))
                .With(previousStep => previousStep.CurrentStepUri)
                .Return(uri => uri.ToString(), defaultRelativeUri);
        }
    }
}
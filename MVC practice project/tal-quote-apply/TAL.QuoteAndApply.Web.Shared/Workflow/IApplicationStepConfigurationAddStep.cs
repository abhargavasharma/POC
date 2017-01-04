using System.Collections.Generic;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public interface IApplicationStepConfigurationCreate
    {
        IApplicationStepConfigurationAddStep CreateApplicationForProduct(string productCode);
    }

    public interface IApplicationStepConfigurationAddStep : IApplicationStepResolver
    {
        IApplicationStepConfigurationAddStep AddStep(ApplicationStep step);
    }

    public interface IApplicationStepResolver
    {
        string ProductName { get; }
        StepWorkflowContext GetFirstStep();
        StepWorkflowContext Resolve(ApplicationStepContext context);
        IEnumerable<ApplicationStep> ResolveAllValidSteps(ApplicationStepContext context);
    }

    public interface IApplicationStepWorkFlowProvider
    {
        IApplicationStepResolver GetForProduct(string productCode);
    }
   
}
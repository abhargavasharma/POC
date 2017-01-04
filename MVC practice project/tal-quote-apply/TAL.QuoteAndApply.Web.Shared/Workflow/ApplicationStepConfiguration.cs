using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public interface IApplicationStepConfiguration : IApplicationStepResolver
    {
        string ProductCode { get; }
        bool ShowNavigationMenu { get; }
        bool ShowStepCount { get; }
        IApplicationStepConfiguration InitializeWorkflowTree(ApplicationStep step);
        IApplicationStepConfiguration WithStepIndicatorSteps(params IApplicationStep[] steps);
        IApplicationStepConfiguration WithStepIndicatorMenu();
        IApplicationStepConfiguration WithStepCount();
    }

    public class ApplicationStepConfiguration : IApplicationStepConfiguration
    {
        public string ProductCode { get; private set; }
        public string ProductName { get; private set; }
        private ApplicationStep _initialStep;
        public IReadOnlyList<IApplicationStep> StepIndicatorSteps { get; private set; }
        public bool ShowNavigationMenu { get; private set; }
        public bool ShowStepCount { get; private set; }

        public IApplicationStepConfiguration InitializeWorkflowTree(ApplicationStep step)
        {
            _initialStep = step;
            return this;
        }

        public IApplicationStepConfiguration WithStepIndicatorSteps(params IApplicationStep[] steps)
        {
            StepIndicatorSteps = steps.ToList();
            return this;
        }

        public IApplicationStepConfiguration WithStepIndicatorMenu()
        {
            ShowNavigationMenu = true;
            return this;
        }

        public IApplicationStepConfiguration WithStepCount()
        {
            ShowStepCount = true;
            return this;
        }

        public StepWorkflowContext GetFirstStep()
        {
            return new StepWorkflowContext(_initialStep);
        }

        public static IApplicationStepConfiguration CreateApplicationForProduct(string productCode, string productName)
        {
            var instance = new ApplicationStepConfiguration { ProductCode = productCode, ProductName = productName};
            return instance;
        }

        public StepWorkflowContext Resolve(ApplicationStepContext context)
        {
            var currentStep = _initialStep;
            var logicalChoice = currentStep;
            while (currentStep != null)
            {
                if (currentStep.PrerequisitsAreSatisfied(context))
                {
                    if (currentStep.IsRequestValidForStep(context))
                    {
                        logicalChoice = currentStep;
                    }
                }
                currentStep = currentStep.GetNextStep(context);               
            }
            if (StepIndicatorSteps != null)
            {
                StepIndicatorSteps.Do(step => step.SetupStepWithContext(context));
                var predicate = new Predicate<IApplicationStep>(step => (context.NumberOfRisks.Equals(2) && !step.IsSingleRiskStepOnly) ||
                            (!step.IsMultiRiskStepOnly && context.NumberOfRisks.Equals(1)));

                return new StepWorkflowContext(logicalChoice,
                    StepIndicatorSteps.Where(predicate.Invoke).ToList().IndexOf(logicalChoice) + 1,
                    StepIndicatorSteps.Where(predicate.Invoke).Select(s => new StepWorkflowItem(s.StepWeight, s.NavigationDisplayText)).ToArray(),
                    ShowNavigationMenu, ShowStepCount);
            }
            return new StepWorkflowContext(logicalChoice);
        }

        public IEnumerable<ApplicationStep> ResolveAllValidSteps(ApplicationStepContext context)
        {
            var step = _initialStep;
            while (step != null)
            {
                if (step.PrerequisitsAreSatisfied(context))
                {
                    yield return step;
                    step = step.GetNextStep(context);
                }
                else
                {
                    step = null;
                }
            }
        }
    }
}
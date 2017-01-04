using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using System.Web;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public interface IApplicationStep : IAddStep
    {
        ApplicationStep GetNextStep(ApplicationStepContext context);
        ApplicationStep GetPreviousStep(ApplicationStepContext context);
        bool IsMultiRiskStepOnly { get; }
        bool IsSingleRiskStepOnly { get; }
        string StepName { get; }
        string NavigationDisplayText { get; }
        string ViewLocation { get; }
        int StepWeight { get; }
        void SetupStepWithContext(ApplicationStepContext context);
    }

    public interface IWithCondition : IAddStep
    {
        IApplicationStep When(Predicate<ApplicationStepContext> condition);
    }

    public interface IAddStep
    {
        IWithCondition WithNextStep(IApplicationStep step);
        IWithCondition WithPreviousStep(IApplicationStep step);
    }

    public abstract class ApplicationStep : IRule<ApplicationStepContext>, IApplicationStep
    {
        private List<ApplicationStepPredicate> Previous { get; set; }
        private List<ApplicationStepPredicate> Next { get; set; }
        
        public int StepWeight { get; private set; }
        public bool IsSingleRiskStepOnly { get; protected set; }
        public string StepName { get; protected set; }
        public string ViewLocation { get; protected set; }
        public bool IsMultiRiskStepOnly { get; protected set; }

        public string NavigationDisplayText { get; protected set; }

        public SimpleUri CurrentStepUri { get; protected set; }
        private RuleResult? _ruleResult;

        protected ApplicationStep()
        {
            Previous = new List<ApplicationStepPredicate>();
            Next = new List<ApplicationStepPredicate>();
            CurrentStepUri = new SimpleUri("");
            StepWeight = 1; // Default Value
        }

        public IWithCondition WithNextStep(IApplicationStep step)
        {
            var appSteoPred = new ApplicationStepPredicate(step, this);
            Next.Add(appSteoPred);
            return appSteoPred;
        }

        public IWithCondition WithPreviousStep(IApplicationStep step)
        {
            var appSteoPred = new ApplicationStepPredicate(step, this);
            Previous.Add(appSteoPred);
            return appSteoPred;
        }

        public ApplicationStep WithWeight(int weight)
        {
            StepWeight = weight;
            return this;
        }

        public ApplicationStep GetNextStep(ApplicationStepContext context)
        {
            return Next.FirstOrDefault(step => step.IsSatisfiedBy(context)).With(step => step.ApplicationStep);
        }

        public ApplicationStep GetPreviousStep(ApplicationStepContext context)
        {
            return Previous.FirstOrDefault(step => step.IsSatisfiedBy(context)).With(step => step.ApplicationStep);
        }

        public abstract bool ValidateStep(ApplicationStepContext context);

        public abstract bool CanArriveToThisStepBackwards(ApplicationStepContext context);

        public virtual void SetupStepWithContext(ApplicationStepContext context)
        {
            // Do nothing. This step is optional
        }

        public virtual void SetupViewLocation(ApplicationStepContext context)
        {
            // Do nothing. This step is optional
        }

        public RuleResult IsSatisfiedBy(ApplicationStepContext context)
        {
            if (!_ruleResult.HasValue)
            {
                SetupStepWithContext(context);
                SetupViewLocation(context);
                var preResult = PrerequisitsAreSatisfied(context);
                _ruleResult = new RuleResult(preResult, "Prerequisite failed");
            }

            if (_ruleResult.Value)
            {
                _ruleResult = new RuleResult(ValidateStep(context));
            }

            return _ruleResult.Value;
        }

        public virtual bool IsRequestValidForStep(ApplicationStepContext context)
        {
            return context.HttpContext.With(httpCtx => httpCtx.Request)
                .With(rq => rq.Url)
                .With(url => url.PathAndQuery)
                .With(VerifyUrlComponents);
        }

        private bool VerifyUrlComponents(string pathAndQuery)
        {
            string path = "", query = "";
            pathAndQuery.Split(new[] { "?" }, StringSplitOptions.None)
                .Do(arr => { path = arr[0]; })
                .If(arr => arr.Length == 2)
                .Do(arr => { query = arr[1]; });

            var queryParams = HttpUtility.ParseQueryString(query ?? "");
            var cStepQueryParams = HttpUtility.ParseQueryString(CurrentStepUri.Query ?? "");
            var allParamsMatch = true;
            foreach (string key in cStepQueryParams)
            {
                allParamsMatch = allParamsMatch && queryParams[key] != null && queryParams[key] == cStepQueryParams[key];
            }
            var currentStepUriPath = CurrentStepUri.Path.Split(new[] {'#'}).First();
            return path.Split('/').LastOrDefault().With(uriComponent => uriComponent.Equals(currentStepUriPath, StringComparison.OrdinalIgnoreCase) && allParamsMatch);
        }

        public bool PrerequisitsAreSatisfied(ApplicationStepContext context)
        {
            var previous = GetPreviousStep(context);
            if (previous != null)
            {
                return previous.IsSatisfiedBy(context) && previous.PrerequisitsAreSatisfied(context);
            }
            return true;
        }

    }
}
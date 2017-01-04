using System;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public class ApplicationStepPredicate : IWithCondition
    {
        public Predicate<ApplicationStepContext> Predicate { get; set; }
        public ApplicationStep ApplicationStep { get; private set; }
        private readonly IApplicationStep _caller;

        public ApplicationStepPredicate(IApplicationStep step, IApplicationStep caller)
        {
            ApplicationStep = step as ApplicationStep;
            _caller = caller;
        }

        public ApplicationStepPredicate(ApplicationStep step, Predicate<ApplicationStepContext> predicate)
        {
            ApplicationStep = step;
            Predicate = predicate;
        }

        public bool IsSatisfiedBy(ApplicationStepContext context)
        {
            if (Predicate == null)
                return true;

            return Predicate.Invoke(context);
        }

        public IApplicationStep When(Predicate<ApplicationStepContext> condition)
        {
            Predicate = condition;
            return _caller;
        }

        public IWithCondition WithNextStep(IApplicationStep step)
        {
            return _caller.WithNextStep(step);
        }

        public IWithCondition WithPreviousStep(IApplicationStep step)
        {
            return _caller.WithPreviousStep(step);
        }
    }
}
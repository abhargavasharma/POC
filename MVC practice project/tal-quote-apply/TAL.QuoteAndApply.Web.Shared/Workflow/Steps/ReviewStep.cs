namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class ReviewStep : ApplicationStep
    {
        public ReviewStep()
        {
            CurrentStepUri.Path = "Review";
        }

        public override bool ValidateStep(ApplicationStepContext context)
        {
            return true;
        }

        public override bool CanArriveToThisStepBackwards(ApplicationStepContext context)
        {
            return true;
        }
    }
}
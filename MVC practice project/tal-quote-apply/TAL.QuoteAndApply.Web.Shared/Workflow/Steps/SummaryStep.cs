namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class SummaryStep : ApplicationStep
    {
        public SummaryStep()
        {
            CurrentStepUri.Path = "Summary";
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
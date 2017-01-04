namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class ReferStep : ApplicationStep
    {
        public ReferStep()
        {
            CurrentStepUri.Path = "submission";
        }

        public override bool ValidateStep(ApplicationStepContext context)
        {
            return true;
        }

        public override bool CanArriveToThisStepBackwards(ApplicationStepContext context)
        {
            return false;
        }
    }
}
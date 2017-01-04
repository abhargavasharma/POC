namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class AlwaysInvalidStep : ApplicationStep
    {
        public override bool ValidateStep(ApplicationStepContext context)
        {
            return false;
        }

        public override bool CanArriveToThisStepBackwards(ApplicationStepContext context)
        {
            return false;
        }
    }
}
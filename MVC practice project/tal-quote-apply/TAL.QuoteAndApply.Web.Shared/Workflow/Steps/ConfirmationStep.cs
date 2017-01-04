namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class ConfirmationStep : ApplicationStep
    {
        public ConfirmationStep()
        {
            CurrentStepUri.Path = "confirmation";
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
namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class HelpMeChooseStep : ApplicationStep
    {
        public HelpMeChooseStep()
        {
            CurrentStepUri.Path = "NeedsAnalysis";
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
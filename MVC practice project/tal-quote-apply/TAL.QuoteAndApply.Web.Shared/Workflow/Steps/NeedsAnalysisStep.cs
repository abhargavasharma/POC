namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class NeedsAnalysisStep : ApplicationStep
    {
        public NeedsAnalysisStep()
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
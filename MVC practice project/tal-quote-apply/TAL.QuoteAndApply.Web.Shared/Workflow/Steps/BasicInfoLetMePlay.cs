namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class BasicInfoLetMePlayStep : ApplicationStep
    {
        public BasicInfoLetMePlayStep()
        {
            CurrentStepUri.Path = "BasicInfo";
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
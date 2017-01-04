using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class PurchaseStep : ApplicationStep
    {
        public PurchaseStep()
        {
            CurrentStepUri.Path = "Purchase";
        }

        public override bool ValidateStep(ApplicationStepContext context)
        {
            return context.Application.Status == PolicyStatus.RaisedToPolicyAdminSystem;
        }

        public override bool CanArriveToThisStepBackwards(ApplicationStepContext context)
        {
            return true;
        }
    }
}
namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public enum ViewModelPolicyStatus
    {
        Incomplete,
        ReadyForInforce,
        RaisedToPolicyAdminSystem,
        ReferredToUnderwriter,
        Inforce,
        FailedToSendToPolicyAdminSystem,
        FailedDuringPolicyAdminSystemLoad
    }

    public class PolicyStatusViewModel
    {
        public string QuoteReferenceNumber { get; set; }
        public ViewModelPolicyStatus Status { get; set; }
    }
}
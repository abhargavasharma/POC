using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public enum PolicyStatus
    {
        Incomplete,
        ReadyForInforce,
        RaisedToPolicyAdminSystem,
        ReferredToUnderwriter,
        Inforce,
        FailedToSendToPolicyAdminSystem, 
        FailedDuringPolicyAdminSystemLoad 
    }

    public class PolicyStatusParam
    {
        public string QuoteReferenceNumber { get; set; }
        public PolicyStatus Status { get; set; }
        public PolicySaveStatus SaveStatus { get; set; }
        public PolicySource Source { get; set; }
    }
}
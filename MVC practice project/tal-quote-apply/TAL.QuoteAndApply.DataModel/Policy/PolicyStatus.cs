namespace TAL.QuoteAndApply.DataModel.Policy
{
    public enum PolicyStatus
    {
        Incomplete = 1,
        ReferredToUnderwriter = 2,
        ReadyForInforce = 3,
        RaisedToPolicyAdminSystem = 4,
        Inforce = 5,
        FailedToSendToPolicyAdminSystem = 6,
        FailedDuringPolicyAdminSystemLoad = 7
    }
}
namespace TAL.QuoteAndApply.DataModel.Policy
{
    public enum MarketingStatus
    {
        //The order of these is important for cover, plan and risk marketing status dependencies, do not change.
        Unknown = 0,
        Lead = 1,
        Accept = 2,
        Refer = 3,
        Off = 4,
        Ineligible = 5,
        Decline = 6
    }
}

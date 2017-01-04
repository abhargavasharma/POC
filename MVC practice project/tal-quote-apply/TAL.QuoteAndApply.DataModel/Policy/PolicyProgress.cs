namespace TAL.QuoteAndApply.DataModel.Policy
{
    public enum PolicyProgress
    {
        Unknown = 0,
        InProgressPreUw = 1,
        InProgressUwReferral = 2,
        InProgressRecommendation = 3,
        InProgressCantContact = 4,
        ClosedSale = 5,
        ClosedNoSale = 6,
        ClosedTriage = 7,
        ClosedCantContact = 8
    }
}
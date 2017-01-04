namespace TAL.QuoteAndApply.DataModel.Policy
{
    public interface IAnalyticsIdentifiers
    {
        string SitecoreContactId { get; }
    }

    public class AnalyticsIdentifiers : IAnalyticsIdentifiers
    {
        public string SitecoreContactId { get; private set; }

        public AnalyticsIdentifiers(string sitecoreContactId)
        {
            SitecoreContactId = sitecoreContactId;
        }
    }
}

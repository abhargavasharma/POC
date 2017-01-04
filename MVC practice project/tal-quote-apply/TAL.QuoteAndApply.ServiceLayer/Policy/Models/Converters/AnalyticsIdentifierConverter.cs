using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IAnalyticsIdentifiersConverter
    {
        IAnalyticsIdentifiers From(PolicyInitialisationMetadata policyInitialisationMetadata);
    }

    public class AnalyticsIdentifiersConverter : IAnalyticsIdentifiersConverter
    {
        public IAnalyticsIdentifiers From(PolicyInitialisationMetadata policyInitialisationMetadata)
        {
            if (policyInitialisationMetadata == null)
                return null;

            return new AnalyticsIdentifiers(policyInitialisationMetadata.ContactId);
        }
    }
}

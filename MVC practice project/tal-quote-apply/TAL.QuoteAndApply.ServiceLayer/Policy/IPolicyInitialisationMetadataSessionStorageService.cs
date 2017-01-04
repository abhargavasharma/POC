using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyInitialisationMetadataSessionStorageService
    {
        void SaveMetadata(PolicyInitialisationMetadata policyInitialisationMetadata);
        PolicyInitialisationMetadata GetMetaData();
    }
}
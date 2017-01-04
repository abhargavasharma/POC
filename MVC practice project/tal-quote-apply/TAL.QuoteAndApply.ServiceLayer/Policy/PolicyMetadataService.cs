using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyInitialisationMetadataService
    {
        void SetPolicyInitialisationMetadataForSession(PolicyInitialisationMetadata policyInitialisationMetadata);
        PolicyInitialisationMetadata GetPolicyInitialisationMetadataForSession();
    }

    public class PolicyInitialisationMetadataService : IPolicyInitialisationMetadataService
    {
        private readonly IPolicyInitialisationMetadataSessionStorageService _policyInitialisationMetadataSessionStorageService;

        public PolicyInitialisationMetadataService(IPolicyInitialisationMetadataSessionStorageService policyInitialisationMetadataSessionStorageService)
        {
            _policyInitialisationMetadataSessionStorageService = policyInitialisationMetadataSessionStorageService;
        }

        public void SetPolicyInitialisationMetadataForSession(PolicyInitialisationMetadata policyInitialisationMetadata)
        {
            _policyInitialisationMetadataSessionStorageService.SaveMetadata(policyInitialisationMetadata);
        }

        public PolicyInitialisationMetadata GetPolicyInitialisationMetadataForSession()
        {
            return _policyInitialisationMetadataSessionStorageService.GetMetaData();
        }
    }
}
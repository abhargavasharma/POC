using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.AccessControl;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Policy.IntegrationTests
{
    public class PolicyCreator
    {
        public static PolicyDto CreatePolicy(IQuoteReferenceGenerationService quoteReferenceGenerationService = null)
        {
            if (quoteReferenceGenerationService == null)
                quoteReferenceGenerationService = new QuoteReferenceGenerationService();

            var policyConfig = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var currentUserProvider = new MockCurrentUserProvider();
            var httpProvider = new MockHttpProvider();
            var dataLayerExceptionFactory = new DataLayerExceptionFactory();
            var dbItemEncryptionService = new DbItemEncryptionService(new SimpleEncryptionService());
            var policyAccessRepo = new PolicyAccessControlDtoRepository(policyConfig, currentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(httpProvider), new EndRequestOperationCollection(httpProvider));
            var policyAccessCreationService = new PolicyAccessControlCreationService(new DateTimeProvider(), policyAccessRepo, currentUserProvider, new AccessControlTypeProvider());
            var policyDtoRepository = new PolicyDtoRepository(policyConfig, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService, new CachingWrapper(httpProvider));
            var createPolicyService = new CreatePolicyService(policyDtoRepository, quoteReferenceGenerationService,
                new MockLoggingService(), policyAccessCreationService);

            return createPolicyService.CreatePolicy(new PolicyDefaultsProvider(new ProductDefinitionBuilder(new MockProductBrandSettingsProvider())).GetPolicyDefaults(1, "tal", 1), PolicySource.Unknown);
        }
    }
}

using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Data
{
    [TestFixture]
    public class PolicyAnalyticsIdentifierDtoRepositoryTests
    {
        [Test]
        public void Get_Insert_Get_Update()
        {
            var quoteReferenceGenerator = new QuoteReferenceGenerationService();
            var config = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var policyAnalyticsIdentifierRepo = new PolicyAnalyticsIdentifierDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
            var policyRepo = new PolicyDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var originalQuoteReference = quoteReferenceGenerator.RandomQuoteReference();
            var policy = new PolicyDto
            {
                QuoteReference = originalQuoteReference,
                Status = PolicyStatus.Incomplete,
                Progress = PolicyProgress.Unknown,
                BrandId = 1,
                OrganisationId = 1
            };

            var insertedPolicy = policyRepo.InsertPolicy(policy);

            var getPolicyAnalyticsIdentifier1 = policyAnalyticsIdentifierRepo.GetByPolicyId(insertedPolicy.Id);

            Assert.That(getPolicyAnalyticsIdentifier1, Is.Null);

            var insertPolicyAnalyticsIdentifier = new PolicyAnalyticsIdentifierDto
            {
                PolicyId = insertedPolicy.Id
            };

            policyAnalyticsIdentifierRepo.InsertPolicyAnalyticsIdentifier(insertPolicyAnalyticsIdentifier);

            var getPolicyAnalyticsIdentifier2 = policyAnalyticsIdentifierRepo.GetByPolicyId(insertedPolicy.Id);

            Assert.That(getPolicyAnalyticsIdentifier2, Is.Not.Null);
            Assert.That(getPolicyAnalyticsIdentifier2.PolicyId, Is.EqualTo(insertedPolicy.Id));

            getPolicyAnalyticsIdentifier2.SitecoreContactId = "TEST TEST";
            policyAnalyticsIdentifierRepo.UpdatePolicyAnalyticsIdentifier(getPolicyAnalyticsIdentifier2);

            var getPolicyAnalyticsIdentifier3 = policyAnalyticsIdentifierRepo.GetByPolicyId(insertedPolicy.Id);

            Assert.That(getPolicyAnalyticsIdentifier3, Is.Not.Null);
            Assert.That(getPolicyAnalyticsIdentifier3.PolicyId, Is.EqualTo(insertedPolicy.Id));
            Assert.That(getPolicyAnalyticsIdentifier3.SitecoreContactId, Is.EqualTo("TEST TEST"));
        }
    }
}

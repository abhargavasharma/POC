using System;
using System.Linq;
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
    public class PolicyDtoRepositoryTests
    {
        [Test]
        public void Insert_Get_Update_Delete()
        {

            var quoteReferenceGenerator = new QuoteReferenceGenerationService();

            var config = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var repo = new PolicyDtoRepository(config, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));

            var originalQuoteReference = quoteReferenceGenerator.RandomQuoteReference();
            var policy = new PolicyDto
            {
                QuoteReference = originalQuoteReference,
                Status = PolicyStatus.Incomplete,
                Progress = PolicyProgress.Unknown,
                BrandId = 1,
                OrganisationId = 1
            };

            var insertedPolicy = repo.InsertPolicy(policy);

            Assert.That(insertedPolicy.Id, Is.GreaterThan(0));

            var getPolicy = repo.GetPolicy(insertedPolicy.Id);

            Assert.That(getPolicy, Is.Not.Null);
            Assert.That(getPolicy.QuoteReference, Is.EqualTo(originalQuoteReference));

            var updatedQuoteReference = quoteReferenceGenerator.RandomQuoteReference();
            getPolicy.QuoteReference = updatedQuoteReference;

            bool updateResult = true;
            try
            {
                repo.UpdatePolicy(getPolicy);
            }
            catch (Exception ex)
            {
                updateResult = false;
            }
            

            Assert.That(updateResult, Is.True);

            var getPolicy2 = repo.GetPolicy(insertedPolicy.Id);

            Assert.That(getPolicy2.QuoteReference, Is.EqualTo(updatedQuoteReference));

            Console.WriteLine("Inserted policy RV:  " + System.Text.Encoding.Default.GetString(insertedPolicy.RV));
            Console.WriteLine("GetPolicy2 RV:       " + System.Text.Encoding.Default.GetString(getPolicy2.RV));
            Console.WriteLine("Inserted == GetPolicy2:  " + insertedPolicy.RV.SequenceEqual(getPolicy2.RV));

            //should fail, because the row version is not the same for this primary key
            var deleteResult = repo.Delete(insertedPolicy);
            Assert.That(deleteResult, Is.False);

            deleteResult = repo.Delete(getPolicy2);
            Assert.That(deleteResult, Is.True);

            var getPolicy3 = repo.GetPolicy(getPolicy2.Id);

            Assert.That(getPolicy3, Is.Null);
        }
    }
}
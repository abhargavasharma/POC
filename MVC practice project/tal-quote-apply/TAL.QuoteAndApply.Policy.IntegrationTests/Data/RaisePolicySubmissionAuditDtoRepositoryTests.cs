using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Data
{
    [TestFixture]
    public class RaisePolicySubmissionAuditDtoRepositoryTests
    {
        [Test]
        public void Insert_Get()
        {
            var config = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var currentUserProvider = new MockCurrentUserProvider();
            var repo = new RaisePolicySubmissionAuditDtoRepository(config, currentUserProvider, new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            var insertedPolicy = PolicyCreator.CreatePolicy();
            var insertedDto = new RaisePolicySubmissionAuditDto
            {
                PolicyId = insertedPolicy.Id,
                RaisePolicyXml = $"<{Guid.NewGuid()}/>"
            };

            var insertResult = repo.InsertRaisePolicySubmissionAudit(insertedDto);

            var getDto = repo.Get(insertResult.Id);

            Assert.That(getDto, Is.Not.Null);
            Assert.That(getDto.PolicyId, Is.EqualTo(insertedDto.PolicyId));
            Assert.That(getDto.RaisePolicyXml, Is.EqualTo(insertedDto.RaisePolicyXml));
        }
    }
}

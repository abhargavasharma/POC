using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Services
{
    [TestFixture]
    public class NoteServiceTests
    {


        [Test]
        public void AddPolicyNote_WhenCalled_IsSuccessful()
        {
            //Arrange
            var policy = PolicyCreator.CreatePolicy();
            var noteService = GetService();

            //Act
            var noteDto = noteService.CreatePolicyNote(policy.QuoteReference, "Note Service Integration Test Note");

            //Assert
            Assert.That(noteDto, Is.Not.Null);
            Assert.That(noteDto.Id, Is.GreaterThan(0));
            Assert.That(noteDto.PolicyId, Is.EqualTo(policy.Id));
            Assert.That(noteDto.NoteText, Is.EqualTo("Note Service Integration Test Note"));
        }

        [Test]
        public void UpdatePolicyNote_WhenCalled_IsSuccessful()
        {
            //Arrange
            var policy = PolicyCreator.CreatePolicy();
            var noteService = GetService();
            var noteDto = noteService.CreatePolicyNote(policy.QuoteReference, "Note Service Integration Test Note");

            //Act
            bool updateResultSuccessful = true;
            try
            {
                noteService.UpdatePolicyNote(noteDto.Id, "Updated Note Service Integration Test Note");
            }
            catch (Exception ex)
            {
                updateResultSuccessful = false;
            }

            //Assert
            Assert.That(updateResultSuccessful, Is.True);            
        }

        [Test]
        public void DeletePolicyNote_WhenCalled_IsSuccessful()
        {
            //Arrange
            var policy = PolicyCreator.CreatePolicy();
            var noteService = GetService();
            var noteDto = noteService.CreatePolicyNote(policy.QuoteReference, "Note Service Integration Test Note");

            //Act
            var deleteResultSuccesful = noteService.DeletePolicyNote(noteDto.Id);

            //Assert
            Assert.That(deleteResultSuccesful, Is.True);
        }

        private static INoteService GetService()
        {
            var policyConfig = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var policyDtoRepository = new PolicyDtoRepository(policyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));
            var noteDtoRepository = new NoteDtoRepository(policyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            return new NoteService(noteDtoRepository, policyDtoRepository);
        }

    }
}

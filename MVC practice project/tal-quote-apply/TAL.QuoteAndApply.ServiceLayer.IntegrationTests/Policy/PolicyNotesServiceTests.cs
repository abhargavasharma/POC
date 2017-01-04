using System;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.Helpers;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy
{
    [TestFixture]
    public class PolicyNotesServiceTests : BaseServiceLayerTest
    {
        [Test]
        public void ProcessPolicyNote_CreatingNote_IsSuccessful_InteractionCreated()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //Arrange
            var policy = PolicyHelper.CreatePolicy();
            var srv = GetServiceInstance<IPolicyNoteService>();            

            //Act
            var result = srv.ProcessPolicyNote(policy.QuoteReference, null, "PolicyNoteService Integration Test Note");

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.NoteText, Is.EqualTo("PolicyNoteService Integration Test Note"));

            var policyInteractionSvc = GetServiceInstance<IPolicyInteractionService>();
            var allInteractions = policyInteractionSvc.GetInteractions(PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(policy.QuoteReference));
            var referredToUnderwriterInteraction = allInteractions.Interactions.FirstOrDefault(x => x.InteractionType == InteractionType.Policy_Note_Added);

            Assert.That(referredToUnderwriterInteraction, Is.Not.Null);
        }

        [Test]
        public void ProcessPolicyNote_UpdateNote_IsSuccessful()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //Arrange
            var policy = PolicyHelper.CreatePolicy();
            var note = NoteHelper.CreateNote(policy.QuoteReference, "PolicyNoteService Integration Test Note");
            var srv = GetServiceInstance<IPolicyNoteService>();

            //Act
            var updatedNoteResult = srv.ProcessPolicyNote(policy.QuoteReference, note.Id, "Updated PolicyNoteService Integration Test Note");

            //Assert
            Assert.That(updatedNoteResult, Is.Not.Null);
            Assert.That(updatedNoteResult.Id, Is.Not.Null);
            Assert.That(updatedNoteResult.Id, Is.GreaterThan(0));
            Assert.That(updatedNoteResult.NoteText, Is.EqualTo("Updated PolicyNoteService Integration Test Note"));
        }

        [Test]
        public void ProcessPolicyNote_DeleteNote_IsSuccessful()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //Arrange
            var policy = PolicyHelper.CreatePolicy();
            var note = NoteHelper.CreateNote(policy.QuoteReference, "PolicyNoteService Integration Test Note");
            var srv = GetServiceInstance<IPolicyNoteService>();

            //Act
            var deleteNoteResult = srv.ProcessPolicyNote(policy.QuoteReference, note.Id, "");

            //Assert
            Assert.That(deleteNoteResult, Is.Not.Null);
            Assert.That(deleteNoteResult.Id, Is.Null);
            Assert.That(deleteNoteResult.NoteText, Is.Null);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void ProcessPolicyNote_InvalidUpdate_ThrowsException()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //Arrange
            var policy = PolicyHelper.CreatePolicy();
            var srv = GetServiceInstance<IPolicyNoteService>();

            //Act
            //Assert
            srv.ProcessPolicyNote(policy.QuoteReference, null, null);
        }

    }
}

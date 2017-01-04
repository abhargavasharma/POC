using System;
using System.Linq;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    [TestFixture]
    public class NoteApiTests: BaseTestClass<NoteClient>
    {
        public NoteApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [Test]
        public async Task EditNote_InsertObjectPassed_ReturnsValidNoteId_Async()
        {
            var policy = PolicyHelper.CreatePolicy();

            var noteUpdateRequest = new NoteUpdateRequest {Id = null, NoteText = "NoteController Integration Test Note"};
            var response = await Client.EditNoteAsync<NoteUpdateResponse>(policy.QuoteReference, noteUpdateRequest);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.NoteId, Is.Not.Null);
            Assert.That(response.NoteId, Is.GreaterThan(0));
        }

        [Test]
        public async Task EditNote_UpdateObjectPassed_ReturnsValidNoteId_Async()
        {
            var policy = PolicyHelper.CreatePolicy();
            var note = NoteHelper.CreateNote(policy.QuoteReference, "NoteController Integration Test Note");

            var noteUpdateRequest = new NoteUpdateRequest { Id = note.Id, NoteText = "Updated NoteController Integration Test Note" };
            var response = await Client.EditNoteAsync<NoteUpdateResponse>(policy.QuoteReference, noteUpdateRequest);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.NoteId, Is.EqualTo(note.Id));
        }

        [Test]
        public async Task EditNote_DeleteObjectPassed_ReturnsNullNoteId_Async()
        {
            var policy = PolicyHelper.CreatePolicy();
            var note = NoteHelper.CreateNote(policy.QuoteReference, "NoteController Integration Test Note");

            var noteUpdateRequest = new NoteUpdateRequest { Id = note.Id, NoteText = null };
            var response = await Client.EditNoteAsync<NoteUpdateResponse>(policy.QuoteReference, noteUpdateRequest);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.NoteId, Is.Null);
        }

        [Test]
        [ExpectedException]
        public async Task EditNote_InvalidQuoteReference_RaisesException_Async()
        {
            var noteUpdateRequest = new NoteUpdateRequest { Id = null, NoteText = "NoteController Integration Test Note" };
            await Client.EditNoteAsync<NoteUpdateResponse>("ImNotAValidQuoteRef", noteUpdateRequest);
        }

        [Test]
        public async Task GetNote_InsertNoteAndQueryTheNote_ReturnsTheNoteForThatQuoteRef_Async()
        {
            //Arrange
            var policy = PolicyHelper.CreatePolicy();
            string noteText = "NoteController Integration Test - Testing GetNotes";
            var noteCreateRequest = new NoteUpdateRequest { Id = null, NoteText = noteText };
            
            //Act
            //Creating a note
            var response = await Client.EditNoteAsync<NoteUpdateResponse>(policy.QuoteReference, noteCreateRequest);
            //Getting the note for that quote ref number
            var resultNote = await Client.GetNoteAsync(policy.QuoteReference);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.NoteId, Is.Not.Null);
            Assert.That(response.NoteId, Is.GreaterThan(0));
            Assert.That(resultNote.PolicyNoteDetailsList.ToList()[0].NoteText, Is.Not.Null);
            Assert.That(resultNote.PolicyNoteDetailsList.ToList()[0].NoteText, Is.EqualTo(noteText));
        }
    }
}

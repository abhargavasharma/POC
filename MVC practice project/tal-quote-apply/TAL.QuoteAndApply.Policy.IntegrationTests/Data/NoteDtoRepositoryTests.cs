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
    public class NoteDtoRepositoryTests
    {

        [Test]
        public void Insert_Get()
        {
            var noteRepo = GetNoteRepository();
            var policy = PolicyCreator.CreatePolicy();

            var noteDto = new NoteDto
            {
                PolicyId = policy.Id,
                NoteText = "Integration Data Test Note"
            };

            var insertedNote = noteRepo.InsertNote(noteDto);
            Assert.That(insertedNote.PolicyId, Is.Not.Null);
            Assert.That(insertedNote.Id, Is.Not.Null);
            Assert.That(insertedNote.NoteText, Is.Not.Null);

            var retrievedNote = noteRepo.GetNote(insertedNote.Id);
            Assert.That(retrievedNote.PolicyId, Is.EqualTo(policy.Id));
            Assert.That(retrievedNote.Id, Is.EqualTo(insertedNote.Id));
            Assert.That(retrievedNote.NoteText, Is.EqualTo("Integration Data Test Note"));
        }

        [Test]
        public void Insert_Update_Get()
        {
            var noteRepo = GetNoteRepository();
            var policy = PolicyCreator.CreatePolicy();

            var noteDto = new NoteDto
            {
                PolicyId = policy.Id,
                NoteText = "Integration Data Test Note"
            };

            var insertedNote = noteRepo.InsertNote(noteDto);
            insertedNote.NoteText = "Updated Integration Data Test Note";
            noteRepo.UpdateNote(insertedNote);

            var retrievedNote = noteRepo.GetNote(insertedNote.Id);

            Assert.That(retrievedNote.PolicyId, Is.EqualTo(policy.Id));
            Assert.That(retrievedNote.Id, Is.EqualTo(insertedNote.Id));
            Assert.That(retrievedNote.NoteText, Is.EqualTo("Updated Integration Data Test Note"));
        }

        [Test]
        public void Insert_Delete()
        {
            var noteRepo = GetNoteRepository();
            var policy = PolicyCreator.CreatePolicy();

            var noteDto = new NoteDto
            {
                PolicyId = policy.Id,
                NoteText = "Integration Data Test Note"
            };

            var insertedNote = noteRepo.InsertNote(noteDto);
            noteRepo.DeleteNote(insertedNote);

            var retrievedNote = noteRepo.GetNote(insertedNote.Id);

            Assert.That(retrievedNote, Is.Null);
        }

        private NoteDtoRepository GetNoteRepository()
        {
            var policyConfig = new PolicyConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var noteRepo = new NoteDtoRepository(policyConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
            return noteRepo;
        }
    }
}

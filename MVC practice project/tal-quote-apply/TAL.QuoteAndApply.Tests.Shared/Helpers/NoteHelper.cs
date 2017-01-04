using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Tests.Shared.Helpers
{
    public static class NoteHelper
    {
        private static readonly INoteService _noteService;

        static NoteHelper()
        {
            _noteService = new NoteService(GetNoteRepository(), PolicyHelper.GetPolicyRepository());
        }

        public static INoteDtoRepository GetNoteRepository()
        {
            var policyConfigurationProvider = new PolicyConfigurationProvider();
            var mockCurrentUserProvider = new MockCurrentUserProvider();
            return new NoteDtoRepository(policyConfigurationProvider, mockCurrentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));
        }

        public static INote CreateNote(string quoteReference, string noteText)
        {
            return _noteService.CreatePolicyNote(quoteReference, noteText);
        }
    }
}

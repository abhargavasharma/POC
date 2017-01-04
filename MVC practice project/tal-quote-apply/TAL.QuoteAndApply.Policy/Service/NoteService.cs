using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface INoteService
    {
        INote CreatePolicyNote(string quoteReference, string noteText);
        void UpdatePolicyNote(int noteId, string noteText);
        bool DeletePolicyNote(int noteId);
        IEnumerable<INote> GetNotesByPolicyId(int policyId);
    }

    public class NoteService : INoteService
    {
        private readonly INoteDtoRepository _noteDtoRepository;
        private readonly IPolicyDtoRepository _policyDtoRepository;

        public NoteService(INoteDtoRepository noteDtoRepository, IPolicyDtoRepository policyDtoRepository)
        {
            _noteDtoRepository = noteDtoRepository;
            _policyDtoRepository = policyDtoRepository;
        }

        public INote CreatePolicyNote(string quoteReference, string noteText)
        {
            var policy = _policyDtoRepository.GetPolicyByQuoteReference(quoteReference);
            var note = _noteDtoRepository.InsertNote(new NoteDto(policy.Id, noteText));
            return note;
        }

        public void UpdatePolicyNote(int noteId, string noteText)
        {
            var existingNote = _noteDtoRepository.GetNote(noteId);
            existingNote.NoteText = noteText;

            _noteDtoRepository.UpdateNote(existingNote);
        }

        public bool DeletePolicyNote(int noteId)
        {
            var existingNote = _noteDtoRepository.GetNote(noteId);
            return _noteDtoRepository.DeleteNote(existingNote);            
        }

        public IEnumerable<INote> GetNotesByPolicyId(int id)
        {
            return _noteDtoRepository.GetNotesByPolicyId(id);
        }
    }
}

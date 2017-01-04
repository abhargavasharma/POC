using System;
using System.Linq;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyNoteService
    {
        PolicyNoteResult ProcessPolicyNote(string quoteReference, int? noteId, string noteText);
        PolicyNoteResult GetNotesByPolicyId(PolicyNotesRequest policyNotesRequest);
    }

    public class PolicyNoteService : IPolicyNoteService
    {
        private readonly INoteService _noteService;
        private readonly IPolicyService _policyService;
        private readonly IPolicyNotesResultConverter _policyNotesResultConverter;
        private readonly IPolicyInteractionService _policyInteractionService;

        public PolicyNoteService(INoteService noteService, 
                                IPolicyService policyService, 
                                IPolicyNotesResultConverter policyNotesResultConverter, 
                                IPolicyInteractionService policyInteractionService)
        {
            _noteService = noteService;
            _policyService = policyService;
            _policyNotesResultConverter = policyNotesResultConverter;
            _policyInteractionService = policyInteractionService;
        }

        public PolicyNoteResult ProcessPolicyNote(string quoteReference, int? noteId, string noteText)
        {
            var hasNoteId = noteId != null;
            var anyNoteText = !string.IsNullOrEmpty(noteText);

            //Create
            if (!hasNoteId && anyNoteText)
            {
                var insertedNote = _noteService.CreatePolicyNote(quoteReference, noteText);
                _policyInteractionService.PolicyNoteAdded(quoteReference);
                return new PolicyNoteResult(insertedNote.Id, noteText);
            }
                
            //Update
            if (hasNoteId && anyNoteText)
            {
                _noteService.UpdatePolicyNote(noteId.Value, noteText);
                return new PolicyNoteResult(noteId.Value, noteText);
            }

            //Delete
            if (hasNoteId && !anyNoteText)
            {
                _noteService.DeletePolicyNote(noteId.Value);
                return new PolicyNoteResult(null, null);
            }

            throw new ApplicationException("Didn't do anything when processing policy note");
        }

        public PolicyNoteResult GetNotesByPolicyId(PolicyNotesRequest policyNotesRequest)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(policyNotesRequest.QuoteReferenceNumber);
            var notes = _noteService.GetNotesByPolicyId(policy.Id);
            return new PolicyNoteResult(notes.Select(_policyNotesResultConverter.From).ToList());
        }
    }
}

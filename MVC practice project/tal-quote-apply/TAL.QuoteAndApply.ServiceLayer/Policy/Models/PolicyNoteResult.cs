using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyNoteResult
    {
        public IReadOnlyList<PolicyNotes> Notes { get; }
        public PolicyNoteResult(IReadOnlyList<PolicyNotes> notes)
        {
            Notes = notes;
        }

        public PolicyNoteResult(int? id, string noteText)
        {
            Id = id;
            NoteText = noteText;
        }

        public int? Id { get; private set; }
        public string NoteText { get; private set; }
    }
}

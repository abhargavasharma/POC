using System;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyNotes
    {
        public DateTime DateCreated { get; }
        public string NoteText { get; }
        public string CreatedBy { get; }

        public PolicyNotes(DateTime dateCreated, string noteText, string createdBy)
        {
            DateCreated = dateCreated;
            NoteText = noteText;
            CreatedBy = createdBy;
        }
    }
}

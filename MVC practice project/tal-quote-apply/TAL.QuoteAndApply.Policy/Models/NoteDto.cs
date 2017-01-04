using System;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface INote
    {
        int Id { get; }
        int PolicyId { get; set; }
        string NoteText { get; set; }
        DateTime CreatedTS { get; set; }
        string CreatedBy { get; set; }
    }

    public class NoteDto : DbItem, INote
    {
        public int PolicyId { get; set; }
        public string NoteText { get; set; }

        public NoteDto() { } //Default Constructor for Dapper

        public NoteDto(int policyId, string noteText)
        {
            PolicyId = policyId;
            NoteText = noteText;
        }
    }
   
}

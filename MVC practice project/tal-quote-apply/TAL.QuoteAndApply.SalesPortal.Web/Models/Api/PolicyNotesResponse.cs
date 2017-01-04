using System.Collections.Generic;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class PolicyNotesResponse
    {
        public PolicyNotesResponse()
        {
            PolicyNoteDetailsList = new List<PolicyNoteDetails>();
        }

        public IEnumerable<PolicyNoteDetails> PolicyNoteDetailsList { get; set; }
    }

    public class PolicyNoteDetails
    {
        public string DateCreated { get; set; }
        public string NoteText { get; set; }
        public string CreatedBy { get; set; }
    }
}
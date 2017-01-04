using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyNotesResultConverter
    {
        PolicyNotes From(INote policyNoteResult);
    }

    public class PolicyNotesResultConverter : IPolicyNotesResultConverter
    {
        public PolicyNotes From(INote policyNoteResult)
        {
            return new PolicyNotes(policyNoteResult.CreatedTS, policyNoteResult.NoteText, policyNoteResult.CreatedBy);
        }
    }
}
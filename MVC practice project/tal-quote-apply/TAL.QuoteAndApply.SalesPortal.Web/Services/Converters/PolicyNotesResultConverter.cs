using System.Linq;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPolicyNotesResultConverter
    {
        PolicyNotesResponse From(PolicyNoteResult policyNotesResult);
    }

    public class PolicyNotesResultConverter : IPolicyNotesResultConverter
    {
        public PolicyNotesResponse From(PolicyNoteResult policyNotesResult)
        {
            return new PolicyNotesResponse
            {
                PolicyNoteDetailsList = policyNotesResult.Notes.Select(From)
            };
        }

        private PolicyNoteDetails From(PolicyNotes policyNotes)
        {
            return new PolicyNoteDetails
            {
                DateCreated = policyNotes.DateCreated.ToFriendlyDateTimeString(),
                NoteText = policyNotes.NoteText,
                CreatedBy = policyNotes.CreatedBy
            };
        }
    }
}
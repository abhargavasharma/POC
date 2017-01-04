namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class NoteUpdateResponse
    {
        public NoteUpdateResponse(int? noteId)
        {
            NoteId = noteId;
        }

        public int? NoteId { get; private set; }
    }
}
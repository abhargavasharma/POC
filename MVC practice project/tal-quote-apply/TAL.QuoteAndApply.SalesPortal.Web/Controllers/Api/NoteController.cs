using System.Web.Http;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using IPolicyNotesResultConverter = TAL.QuoteAndApply.SalesPortal.Web.Services.Converters.IPolicyNotesResultConverter;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    [RoutePrefix("api/policy/{quoteReference}")]
    public class NoteController : ApiController
    {

        private readonly IPolicyNoteService _policyNoteService;
        private readonly IPolicyNotesRequestConverter _policyNotesRequestConverter;
        private readonly IPolicyNotesResultConverter _policyNotesResultConverter;

        public NoteController(IPolicyNoteService policyNoteService,
                              IPolicyNotesRequestConverter policyNotesRequestConverter,
                              IPolicyNotesResultConverter policyNotesResultConverter)
        {
            _policyNoteService = policyNoteService;
            _policyNotesRequestConverter = policyNotesRequestConverter;
            _policyNotesResultConverter = policyNotesResultConverter;
        }

        [HttpGet, Route("notes")]
        public IHttpActionResult GetNotes(string quoteReference)
        {
            var request = _policyNotesRequestConverter.From(quoteReference);
            var searchResult = _policyNoteService.GetNotesByPolicyId(request);
            return Ok(_policyNotesResultConverter.From(searchResult));
        }

        [HttpPost, Route("note")]
        public IHttpActionResult UpdateNote(string quoteReference, NoteUpdateRequest noteUpdateRequest)
        {
            
            if (!ModelState.IsValid) //Note: keeping model state valid test for consistancy. NoteUpdateRequest doesn't have validation attibutes at the moment so this will never be true
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var noteResult = _policyNoteService.ProcessPolicyNote(quoteReference, noteUpdateRequest.Id,
                noteUpdateRequest.NoteText);

            return Ok(new NoteUpdateResponse(noteResult.Id));
        }

        

    }
}
using System.Web.Http;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.ServiceLayer.Search.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.Customer.Web.Controllers.Api
{
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private readonly ISingleLevelDrillDownSearchUnderwritingQuestionAnswerService _searchUnderwritingQuestionAnswerService;
        private readonly ISearchQuotesClientsAndProspectsService _searchQuotesClientsAndProspectsService;
        
        public SearchController(ISingleLevelDrillDownSearchUnderwritingQuestionAnswerService searchUnderwritingQuestionAnswerService,
            ISearchQuotesClientsAndProspectsService searchQuotesClientsAndProspectsService)
        {
            _searchUnderwritingQuestionAnswerService = searchUnderwritingQuestionAnswerService;
            _searchQuotesClientsAndProspectsService = searchQuotesClientsAndProspectsService;
        }

        [HttpGet, Route("question/occupation")]
        public IHttpActionResult SearchOccupationQuestionAnswers(string query, int limit = 10)
        {
            var templateVersion = _searchUnderwritingQuestionAnswerService.EnsureIndexForDrillDownAndGetTemplateVersion(
                    QuestionTagConstants.IndustryQuestionTag, QuestionTagConstants.OccupationQuestionTag);
            var questionHashCode = SearchQuestionHashProvider.CreateHashKeyFor(QuestionTagConstants.IndustryQuestionTag);
            var results = _searchUnderwritingQuestionAnswerService.Search(questionHashCode, templateVersion, query, limit);

            return Ok(results);
        }
        
    }
    
}

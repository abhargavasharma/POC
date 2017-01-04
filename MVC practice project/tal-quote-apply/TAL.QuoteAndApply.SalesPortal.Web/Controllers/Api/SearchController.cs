using System.Linq;
using System.Web.Http;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.ServiceLayer.Search.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [WebApiSalesPortalSessionRequired]
    public class SearchController : ApiController
    {
        private readonly ISingleLevelDrillDownSearchUnderwritingQuestionAnswerService _searchUnderwritingQuestionAnswerService;
        private readonly ISearchQuotesClientsAndProspectsService _searchQuotesClientsAndProspectsService;
        private readonly ISalesPortalSessionContext _salesPortalSessionContext;
        private readonly IProductBrandProvider _productBrandProvider;

        private readonly ISearchQuotesClientsAndProspectsRequestConverter
            _searchQuotesClientsAndProspectsRequestConverter;
        
        private readonly ISearchQuotesClientsAndProspectsResultConverter _searchQuotesClientsAndProspectsResultConverter;

        public SearchController(ISingleLevelDrillDownSearchUnderwritingQuestionAnswerService searchUnderwritingQuestionAnswerService,
            ISearchQuotesClientsAndProspectsService searchQuotesClientsAndProspectsService,
            ISearchQuotesClientsAndProspectsRequestConverter searchQuotesClientsAndProspectsRequestConverter,
            ISearchQuotesClientsAndProspectsResultConverter searchQuotesClientsAndProspectsResultConverter, 
            ISalesPortalSessionContext salesPortalSessionContext, IProductBrandProvider productBrandProvider)
        {
            _searchUnderwritingQuestionAnswerService = searchUnderwritingQuestionAnswerService;
            _searchQuotesClientsAndProspectsService = searchQuotesClientsAndProspectsService;
            _searchQuotesClientsAndProspectsRequestConverter = searchQuotesClientsAndProspectsRequestConverter;
            _searchQuotesClientsAndProspectsResultConverter = searchQuotesClientsAndProspectsResultConverter;
            _salesPortalSessionContext = salesPortalSessionContext;
            _productBrandProvider = productBrandProvider;
        }

        [HttpGet, Route("api/search/question/occupation")]
        public IHttpActionResult SearchOccupationQuestionAnswers(string query, int limit = 10)
        {
            var templateVersion = _searchUnderwritingQuestionAnswerService.EnsureIndexForDrillDownAndGetTemplateVersion(
                    QuestionTagConstants.IndustryQuestionTag, QuestionTagConstants.OccupationQuestionTag);
            var questionHashCode = SearchQuestionHashProvider.CreateHashKeyFor(QuestionTagConstants.IndustryQuestionTag);
            var results = _searchUnderwritingQuestionAnswerService.Search(questionHashCode, templateVersion, query, limit);

            return Ok(results);
        }

        [HttpPost, Route("api/search/clients")]
        public IHttpActionResult SearchClients(SearchClientsRequest searchClientsRequest)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var request = _searchQuotesClientsAndProspectsRequestConverter.From(searchClientsRequest);

            var salesPortalSession = _salesPortalSessionContext.SalesPortalSession;
            request.BrandId = salesPortalSession.Roles.Contains(Role.Underwriter) ? (int?)null : _productBrandProvider.GetBrandIdByKey(salesPortalSession.SelectedBrand);
            var searchResult = _searchQuotesClientsAndProspectsService.Search(request);

            return Ok(_searchQuotesClientsAndProspectsResultConverter.From(searchResult));
        }
    }
}

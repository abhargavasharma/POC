using System.Collections.Generic;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;
using TAL.QuoteAndApply.ServiceLayer.Search.Underwriting;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using SearchResultType = TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models.SearchResultType;

namespace TAL.QuoteAndApply.SalesPortal.Web.UnitTests.Controllers.Api
{
    [TestFixture]
    public class SearchControllerTests
    {
        private Mock<ISingleLevelDrillDownSearchUnderwritingQuestionAnswerService> _mockSearchUnderwritingQuestionAnswerService;
        private Mock<ISearchQuotesClientsAndProspectsService> _mockSearchQuotesClientsAndProspectsService;
        private Mock<ISearchQuotesClientsAndProspectsRequestConverter> _mockSearchQuotesClientsAndProspectsRequestConverter;
        private Mock<ISearchQuotesClientsAndProspectsResultConverter> _mockSearchQuotesClientsAndProspectsResultConverter;
        private Mock<ISalesPortalSessionContext> _mockSalesPortalSessionContext;
        private Mock<IProductBrandProvider> _mockProductBrandProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            _mockSearchUnderwritingQuestionAnswerService =
                mockRepository.Create<ISingleLevelDrillDownSearchUnderwritingQuestionAnswerService>();

            _mockSearchQuotesClientsAndProspectsService =
                mockRepository.Create<ISearchQuotesClientsAndProspectsService>();

            _mockSearchQuotesClientsAndProspectsRequestConverter = mockRepository.Create<ISearchQuotesClientsAndProspectsRequestConverter>();
            _mockSearchQuotesClientsAndProspectsResultConverter = mockRepository.Create<ISearchQuotesClientsAndProspectsResultConverter>();
            _mockSalesPortalSessionContext = mockRepository.Create<ISalesPortalSessionContext>();
            _mockProductBrandProvider = mockRepository.Create<IProductBrandProvider>();
        }


        [Test]
        public void SearchClients_ModelStateInvalid_InvalidModelStateResultReturned()
        {
            var searchClientsRequest = new SearchClientsRequest {QuoteReferenceNumber = "123456789"};

            var ctrl = GetController();

            ctrl.ModelState.AddModelError("Test", "Test");

            var result = ctrl.SearchClients(searchClientsRequest);

            Assert.That(result, Is.TypeOf<InvalidModelStateActionResult>());

            var invalidModelStateResult = result as InvalidModelStateActionResult;

            Assert.That(invalidModelStateResult, Is.Not.Null);
        }


        [Test]
        public void SearchClients_ValidForSearch_OkReturned()
        {
            var searchClientsRequest = new SearchClientsRequest { LeadId = "12" };
            var mockSearchQuotesClientsAndProspectsRequest = SearchQuotesClientsAndProspectsRequest.QuoteReferenceSearch(searchClientsRequest.QuoteReferenceNumber);
            var mockSearchQuotesClientsAndProspectsResponse = new SearchClientsAndProspectsResult(new List<SearchResult>(), SearchResultType.Quotes);
            var searchClientsResponse = new SearchClientsResponse();

            _mockSearchQuotesClientsAndProspectsRequestConverter.Setup(call => call.From(searchClientsRequest))
                .Returns(mockSearchQuotesClientsAndProspectsRequest);

            _mockSearchQuotesClientsAndProspectsService.Setup(call => call.Search(mockSearchQuotesClientsAndProspectsRequest))
                .Returns(mockSearchQuotesClientsAndProspectsResponse);

            _mockSearchQuotesClientsAndProspectsResultConverter.Setup(
                call => call.From(mockSearchQuotesClientsAndProspectsResponse)).Returns(searchClientsResponse);

            _mockSalesPortalSessionContext.Setup(
                call => call.SalesPortalSession).Returns(new SalesPortalSession("Test", new List<Role>(), "test@test.com", "test", "test", "TAL"));

            _mockProductBrandProvider.Setup(call => call.GetBrandIdByKey(It.IsAny<string>())).Returns(1);

            var ctrl = GetController();

            var result = ctrl.SearchClients(searchClientsRequest);

            Assert.That(result, Is.TypeOf<OkNegotiatedContentResult<SearchClientsResponse>>());
        }
        

        private SearchController GetController()
        {
            return new SearchController(_mockSearchUnderwritingQuestionAnswerService.Object,
                _mockSearchQuotesClientsAndProspectsService.Object,
                _mockSearchQuotesClientsAndProspectsRequestConverter.Object,
                _mockSearchQuotesClientsAndProspectsResultConverter.Object, _mockSalesPortalSessionContext.Object, _mockProductBrandProvider.Object);
        }
    }
}

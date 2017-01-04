using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Search.Clients
{
    [TestFixture]
    public class SearchByLeadIdServiceTests
    {
        private Mock<IPolicySearchService> _mockPolicySearchService;
        private Mock<ISearchQuoteResultProvider> _mockSearchQuoteResultConverter;
        private Mock<IGetLeadService> _mockGetLeadService;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            _mockPolicySearchService = mockRepository.Create<IPolicySearchService>();
            _mockSearchQuoteResultConverter = mockRepository.Create<ISearchQuoteResultProvider>();
            _mockGetLeadService = mockRepository.Create<IGetLeadService>();
        }

        [Test]
        public void Search_LeadIdIsNull_ReturnEmptyList()
        {
            var svc = GetService();

            var result = svc.Search(SearchQuotesClientsAndProspectsRequest.LeadIdSearch(null));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SearchResults, Is.Not.Null);
            Assert.That(result.SearchResults.Any(), Is.False);
        }

        [Test]
        public void Search_RecordFound_SearchResultReturnedList()
        {
            var leadId = 99;

            var policySearchResult = new PolicySearchResultDto()
            {
                QuoteReference = "1234567890",
                Premium = 100m,
                FirstName = "Firstname",
                Surname = "Surname",
                DateOfBirth = DateTime.Today.AddYears(-30),
                State = State.VIC,
                HomeNumber = "0233334444",
                MobileNumber = "0450993315",
                EmailAddress = "test@test.com",
                LeadId = leadId,
                BrandId = 1
            };

            var searchQuoteResult = new SearchResult(
                "1234567890",
                100m,
                leadId,
                "Firstname",
                "Surname",
                DateTime.Today.AddYears(-30),
                 State.VIC,
                 "0450993315",
                 "0233334444",
                "test@test.com",
                "M",
                "TestExteranlRef"
            );

            _mockPolicySearchService.Setup(call => call.SearchByLeadId(It.IsAny<long>())).Returns(new List<IPolicySearchResult> { policySearchResult });
            _mockSearchQuoteResultConverter.Setup(call => call.From(policySearchResult)).Returns(searchQuoteResult);

            var svc = GetService();
            var searchQuotesClientsAndProspectsRequest = SearchQuotesClientsAndProspectsRequest.LeadIdSearch(leadId);
            searchQuotesClientsAndProspectsRequest.BrandId = 1;
            var result = svc.Search(searchQuotesClientsAndProspectsRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SearchResults, Is.Not.Null);
            Assert.That(result.SearchResults.Any(), Is.True);

            var searchResult = result.SearchResults.ToList().First();
            Assert.That(searchResult.LeadId, Is.EqualTo(policySearchResult.LeadId));
            Assert.That(searchResult.QuoteReferenceNumber, Is.EqualTo(policySearchResult.QuoteReference));
            Assert.That(searchResult.Premium, Is.EqualTo(policySearchResult.Premium));
            Assert.That(searchResult.FirstName, Is.EqualTo(policySearchResult.FirstName));
            Assert.That(searchResult.Surname, Is.EqualTo(policySearchResult.Surname));
            Assert.That(searchResult.DateOfBirth, Is.EqualTo(policySearchResult.DateOfBirth));
            Assert.That(searchResult.MobileNumber, Is.EqualTo(policySearchResult.MobileNumber));
            Assert.That(searchResult.HomeNumber, Is.EqualTo(policySearchResult.HomeNumber));
            Assert.That(searchResult.EmailAddress, Is.EqualTo(policySearchResult.EmailAddress));
        }

        private SearchByLeadIdService GetService()
        {
            return new SearchByLeadIdService(_mockPolicySearchService.Object, _mockSearchQuoteResultConverter.Object, _mockGetLeadService.Object);
        }
    }
}

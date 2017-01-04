using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Search.Clients
{
    [TestFixture]
    public class SearchByQuoteReferenceNumberServiceTests
    {
        private Mock<IPolicySearchService> _mockPolicySearchService;
        private Mock<ISearchQuoteResultProvider> _mockSearchQuoteResultConverter;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            _mockPolicySearchService = mockRepository.Create<IPolicySearchService>();
            _mockSearchQuoteResultConverter = mockRepository.Create<ISearchQuoteResultProvider>();
        }

        [Test]
        public void Search_QuoteReferenceNumberIsNull_ReturnEmptyList()
        {
            var svc = GetService();

            var result = svc.Search(SearchQuotesClientsAndProspectsRequest.QuoteReferenceSearch(null));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SearchResults, Is.Not.Null);
            Assert.That(result.SearchResults.Any(), Is.False);
        }

        [Test]
        public void Search_QuoteReferenceNumberIsEmpty_ReturnEmptyList()
        {
            var svc = GetService();

            var result = svc.Search(SearchQuotesClientsAndProspectsRequest.QuoteReferenceSearch(String.Empty));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SearchResults, Is.Not.Null);
            Assert.That(result.SearchResults.Any(), Is.False);
        }
               
        [Test]
        public void Search_RecordFound_SearchResultReturnedList()
        {
            var quoteRef = "1234567890";
            
            var policySearchResult = new PolicySearchResultDto()
            {
                QuoteReference = quoteRef,
                Premium = 100m,
                FirstName = "Firstname",
                Surname = "Surname",
                DateOfBirth = DateTime.Today.AddYears(-30),
                State = State.VIC,
                MobileNumber = "0450993312",
                HomeNumber = "0344446666",
                EmailAddress = "test@test.com",
                BrandId = 1
            };

            var searchQuoteResult = new SearchResult (
                quoteRef,
                100m,
                null,
                "Firstname",
                "Surname",
                DateTime.Today.AddYears(-30),
                 State.VIC,
                 "0450993312",
                "0344446666",
                "test@test.com",
                "F",
                null,
                "TAL"
            );

            _mockPolicySearchService.Setup(call => call.SearchByQuoteReference(It.IsAny<string>())).Returns(new List<IPolicySearchResult> { policySearchResult });
            _mockSearchQuoteResultConverter.Setup(call => call.From(policySearchResult)).Returns(searchQuoteResult);

            var svc = GetService();
            var searchRequest = SearchQuotesClientsAndProspectsRequest.QuoteReferenceSearch(quoteRef);
            searchRequest.BrandId = 1;
            var result = svc.Search(searchRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SearchResults, Is.Not.Null);
            Assert.That(result.SearchResults.Any(), Is.True);

            var searchResult = result.SearchResults.ToList().First();
            Assert.That(searchResult.QuoteReferenceNumber, Is.EqualTo(quoteRef));
            Assert.That(searchResult.Premium, Is.EqualTo(policySearchResult.Premium));
            Assert.That(searchResult.FirstName, Is.EqualTo(policySearchResult.FirstName));
            Assert.That(searchResult.Surname, Is.EqualTo(policySearchResult.Surname));
            Assert.That(searchResult.DateOfBirth, Is.EqualTo(policySearchResult.DateOfBirth));
            Assert.That(searchResult.MobileNumber, Is.EqualTo(policySearchResult.MobileNumber));
            Assert.That(searchResult.HomeNumber, Is.EqualTo(policySearchResult.HomeNumber));
            Assert.That(searchResult.EmailAddress, Is.EqualTo(policySearchResult.EmailAddress));
        }

        [Test]
        public void Search_RecordFoundForExternalCustomer_SearchResultReturnedList()
        {
            var quoteRef = "1234567890";

            var policySearchResult = new PolicySearchResultDto()
            {
                QuoteReference = quoteRef,
                Premium = 100m,
                FirstName = "Firstname",
                Surname = "Surname",
                DateOfBirth = DateTime.Today.AddYears(-30),
                State = State.VIC,
                MobileNumber = "0450993312",
                HomeNumber = "0344446666",
                EmailAddress = "test@test.com",
                ExternalCustomerReference = "123",
                BrandId = 1
            };

            var searchQuoteResult = new SearchResult(
                quoteRef,
                100m,
                null,
                "Firstname",
                "Surname",
                DateTime.Today.AddYears(-30),
                 State.VIC,
                 "0450993312",
                "0344446666",
                "test@test.com",
                "F",
                "123",
                "TAL"
            );

            _mockPolicySearchService.Setup(call => call.SearchByQuoteReference(It.IsAny<string>())).Returns(new List<IPolicySearchResult> { policySearchResult });
            _mockSearchQuoteResultConverter.Setup(call => call.From(policySearchResult)).Returns(searchQuoteResult);

            var svc = GetService();
            var searchRequest = SearchQuotesClientsAndProspectsRequest.QuoteReferenceSearch(quoteRef);
            searchRequest.BrandId = 1;
            var result = svc.Search(searchRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SearchResults, Is.Not.Null);
            Assert.That(result.SearchResults.Any(), Is.True);

            var searchResult = result.SearchResults.ToList().First();
            Assert.That(searchResult.QuoteReferenceNumber, Is.EqualTo(quoteRef));
            Assert.That(searchResult.Premium, Is.EqualTo(policySearchResult.Premium));
            Assert.That(searchResult.FirstName, Is.EqualTo(policySearchResult.FirstName));
            Assert.That(searchResult.Surname, Is.EqualTo(policySearchResult.Surname));
            Assert.That(searchResult.DateOfBirth, Is.EqualTo(policySearchResult.DateOfBirth));
            Assert.That(searchResult.MobileNumber, Is.EqualTo(policySearchResult.MobileNumber));
            Assert.That(searchResult.HomeNumber, Is.EqualTo(policySearchResult.HomeNumber));
            Assert.That(searchResult.EmailAddress, Is.EqualTo(policySearchResult.EmailAddress));
            Assert.That(searchResult.ExternalCustomerReference, Is.EqualTo(policySearchResult.ExternalCustomerReference));
            Assert.That(searchResult.Brand, Is.EqualTo(searchQuoteResult.Brand));
        }
        private SearchByQuoteReferenceNumberService GetService()
        {
            return new SearchByQuoteReferenceNumberService(_mockPolicySearchService.Object, _mockSearchQuoteResultConverter.Object);
        }
    }
}

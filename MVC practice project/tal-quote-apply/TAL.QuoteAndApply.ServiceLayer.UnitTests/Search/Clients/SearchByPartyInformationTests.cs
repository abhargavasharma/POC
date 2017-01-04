using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Search.Clients
{
    [TestFixture]
    public class SearchByPartyInformationTests
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
        public void Search_PartyInfoIsEmpty_ReturnEmptyList()
        {
            var svc = GetService();

            var result = svc.Search(SearchQuotesClientsAndProspectsRequest.PartyInfoSearch(null, null, null, null, null, null));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.SearchResults, Is.Not.Null);
            Assert.That(result.SearchResults.Any(), Is.False);
        }

        [Test]
        public void Search_RecordFound_SearchResultReturnedList()
        {
            var firstName = "Andrew";
            var surname = "Bell";
            var dob = new DateTime(1978, 5, 1);
            var email = "rundy.undy@tal.com.au";
            var mobileNumber = "0400005555";
            var homeNumber = "0700005555";

            var policySearchResult = new PolicySearchResultDto()
            {
                QuoteReference = "1234567890",
                Premium = 100m,
                FirstName = firstName,
                Surname = surname,
                DateOfBirth = dob,
                State = State.VIC,
                MobileNumber = mobileNumber,
                HomeNumber = homeNumber,
                EmailAddress = email,
                LeadId = 3,
                BrandId = 1
            };

            var searchQuoteResult = new SearchResult(
                "1234567890",
                100m,
                3,
                firstName,
                surname,
                dob,
                 State.VIC,
                mobileNumber,
                homeNumber,
                email,
                "M",
                null,
                "TAL"
            );

            _mockPolicySearchService.Setup(call => call.SearchByPolicyOwnerDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), 
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<IPolicySearchResult> { policySearchResult });
            _mockSearchQuoteResultConverter.Setup(call => call.From(policySearchResult)).Returns(searchQuoteResult);

            var svc = GetService();
            var searchQuotesClientsAndProspectsRequest =SearchQuotesClientsAndProspectsRequest.PartyInfoSearch(firstName, surname, dob, mobileNumber, homeNumber, email, "");
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
            Assert.That(searchResult.ExternalCustomerReference, Is.EqualTo(policySearchResult.ExternalCustomerReference));
            Assert.That(searchResult.Brand, Is.EqualTo(searchQuoteResult.Brand));
        }

        [Test]
        public void Search_RecordFoundForExternalCustomer_SearchResultReturnedList()
        {
            var firstName = "Andrew";
            var surname = "Bell";
            var dob = new DateTime(1978, 5, 1);
            var email = "rundy.undy@tal.com.au";
            var mobileNumber = "0400005555";
            var homeNumber = "0700005555";
            var externalCustomerReference = "123";

            var policySearchResult = new PolicySearchResultDto()
            {
                QuoteReference = "1234567890",
                Premium = 100m,
                FirstName = firstName,
                Surname = surname,
                DateOfBirth = dob,
                State = State.VIC,
                MobileNumber = mobileNumber,
                HomeNumber = homeNumber,
                EmailAddress = email,
                LeadId = 3,
                ExternalCustomerReference = "123",
                BrandId = 1
            };

            var searchQuoteResult = new SearchResult(
                "1234567890",
                100m,
                3,
                firstName,
                surname,
                dob,
                 State.VIC,
                mobileNumber,
                homeNumber,
                email,
                "M",
                "123",
                "TAL"
            );

            _mockPolicySearchService.Setup(call => call.SearchByPolicyOwnerDetails(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<IPolicySearchResult> { policySearchResult });
            _mockSearchQuoteResultConverter.Setup(call => call.From(policySearchResult)).Returns(searchQuoteResult);

            var svc = GetService();
            var searchQuotesClientsAndProspectsRequest = SearchQuotesClientsAndProspectsRequest.PartyInfoSearch(firstName, surname, dob, mobileNumber, homeNumber, email, externalCustomerReference);
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
            Assert.That(searchResult.ExternalCustomerReference, Is.EqualTo(policySearchResult.ExternalCustomerReference));
        }
        private SearchByPartyInformationService GetService()
        {
            return new SearchByPartyInformationService(_mockPolicySearchService.Object, _mockSearchQuoteResultConverter.Object);
        }
    }
}

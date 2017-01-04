using System;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Payment.Rules;
using TAL.QuoteAndApply.Payment.Service;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;
using TAL.QuoteAndApply.DataModel.Personal;
using System.Collections.Generic;
using System.Linq;
using SearchResultType = TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models.SearchResultType;

namespace TAL.QuoteAndApply.SalesPortal.Web.UnitTests.Services.Converters
{
    [TestFixture]
    public class SearchQuotesClientsAndProspectsResultConverterTests
    {
        [Test]
        public void From_WhenBothMobileAndHomePhoneNumbersProvided_MobileNumberReturnedAsPhoneNumber()
        {
            //Arrange
            var searchQuotesClientsAndProspectsResultConverter = new SearchQuotesClientsAndProspectsResultConverter();
            var searchQuoteResult = new SearchResult("123", 1000m, 123, "Chris", "Bower",
                new DateTime(1980, 4, 20), State.NSW, "0450993315", "0233336666", "chris@gmail.com", "M", "123")
            {};
            var searchQuoteResultList = new List<SearchResult>();
            searchQuoteResultList.Add(searchQuoteResult);
            var searchQuotesClientsAndProspectsResult = new SearchClientsAndProspectsResult(
                searchQuoteResultList, SearchResultType.Quotes);

            //Act
            var searchClientsResponse = searchQuotesClientsAndProspectsResultConverter.From(searchQuotesClientsAndProspectsResult);
            var result = searchClientsResponse.Quotes.First();

            //Assert
            Assert.That(result.OwnerPhoneNumber,  Is.EqualTo("0450993315"));
            Assert.That(result.OwnerGender, Is.EqualTo("M"));
            Assert.That(result.ExternalCustomerReference, Is.EqualTo("123"));
        }

        [Test]
        public void From_WhenMobileNumberOnlyProvided_MobileNumberReturnedAsPhoneNumber()
        {
            //Arrange
            var searchQuotesClientsAndProspectsResultConverter = new SearchQuotesClientsAndProspectsResultConverter();
            var searchQuoteResult = new SearchResult("123", 1000m, 123, "Chris", "Bower",
                new DateTime(1980, 4, 20), State.NSW, "0450993315", string.Empty, "chris@gmail.com", "F", "123")
            { };
            var searchQuoteResultList = new List<SearchResult>();
            searchQuoteResultList.Add(searchQuoteResult);
            var searchQuotesClientsAndProspectsResult = new SearchClientsAndProspectsResult(
                searchQuoteResultList, SearchResultType.Quotes);

            //Act
            var searchClientsResponse = searchQuotesClientsAndProspectsResultConverter.From(searchQuotesClientsAndProspectsResult);
            var result = searchClientsResponse.Quotes.First();

            //Assert
            Assert.That(result.OwnerPhoneNumber, Is.EqualTo("0450993315"));
            Assert.That(result.OwnerGender, Is.EqualTo("F"));
            Assert.That(result.ExternalCustomerReference, Is.EqualTo("123"));
        }

        [Test]
        public void From_WhenHomeNumberOnlyProvided_HomeNumberReturnedAsPhoneNumber()
        {
            //Arrange
            var searchQuotesClientsAndProspectsResultConverter = new SearchQuotesClientsAndProspectsResultConverter();
            var searchQuoteResult = new SearchResult("123", 1000m, 123, "Chris", "Bower",
                new DateTime(1980, 4, 20), State.NSW, string.Empty, "0233336666", "chris@gmail.com", "M", "123")
            { };
            var searchQuoteResultList = new List<SearchResult>();
            searchQuoteResultList.Add(searchQuoteResult);
            var searchQuotesClientsAndProspectsResult = new SearchClientsAndProspectsResult(
                searchQuoteResultList, SearchResultType.Quotes);

            //Act
            var searchClientsResponse = searchQuotesClientsAndProspectsResultConverter.From(searchQuotesClientsAndProspectsResult);
            var result = searchClientsResponse.Quotes.First();

            //Assert
            Assert.That(result.OwnerPhoneNumber, Is.EqualTo("0233336666"));
            Assert.That(result.ExternalCustomerReference, Is.EqualTo("123"));
        }

        [Test]
        public void From_WhenBothMobileAndHomeNumberAreNullOrEmpty_ResultIsEmpty()
        {
            //Arrange
            var searchQuotesClientsAndProspectsResultConverter = new SearchQuotesClientsAndProspectsResultConverter();
            var searchQuoteResult = new SearchResult("123", 1000m, 123, "Chris", "Bower",
                new DateTime(1980, 4, 20), State.NSW, string.Empty, string.Empty, "chris@gmail.com", "M", "123")
            { };
            var searchQuoteResultList = new List<SearchResult>();
            searchQuoteResultList.Add(searchQuoteResult);
            var searchQuotesClientsAndProspectsResult = new SearchClientsAndProspectsResult(
                searchQuoteResultList, SearchResultType.Quotes);

            //Act
            var searchClientsResponse = searchQuotesClientsAndProspectsResultConverter.From(searchQuotesClientsAndProspectsResult);
            var result = searchClientsResponse.Quotes.First();

            //Assert
            Assert.That(result.OwnerPhoneNumber, Is.Null);
            Assert.That(result.ExternalCustomerReference, Is.EqualTo("123"));
        }
    }
}

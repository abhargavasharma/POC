using System.Linq;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface ISearchQuotesClientsAndProspectsResultConverter
    {
        SearchClientsResponse From(SearchClientsAndProspectsResult seraSearchClientsAndProspectsResult);
    }

    public class SearchQuotesClientsAndProspectsResultConverter : ISearchQuotesClientsAndProspectsResultConverter
    {
        public SearchClientsResponse From(SearchClientsAndProspectsResult searchClientsAndProspectsResult)
        {
            return new SearchClientsResponse
            {
                Quotes = searchClientsAndProspectsResult.SearchResults.Select(From),
                ResultType = searchClientsAndProspectsResult.ResultType
            };
        }

        private QuoteSummary From(SearchResult searchResult)
        {
            return new QuoteSummary
            {
                OwnerDateOfBirth = searchResult.DateOfBirth.ToFriendlyString(),
                OwnerEmailAddress = searchResult.EmailAddress,
                OwnerPhoneNumber = GetPhoneNumber(searchResult.MobileNumber, searchResult.HomeNumber),
                OwnerState = GetStateString(searchResult.State),
                OwnerName = GetName(searchResult.FirstName, searchResult.Surname),
                Premium = GetPremium(searchResult.Premium),
                QuoteReferenceNumber = searchResult.QuoteReferenceNumber,
                LeadId = searchResult.LeadId,
                OwnerGender = searchResult.Gender,
                ExternalCustomerReference = searchResult.ExternalCustomerReference,
                Brand = searchResult.Brand
            };
        }

        private decimal? GetPremium(decimal premium)
        {
            if (premium == 0)
            {
                return null;
            }

            return premium;
        }

        private string GetName(string firstName, string surname)
        {
            string name = null;

            if (!string.IsNullOrEmpty(firstName))
            {
                name = firstName;
            }

            if (!string.IsNullOrEmpty(surname))
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = surname;
                }
                else
                {
                    name = name + " " + surname;
                }
            }

            return name;
        }

        private string GetPhoneNumber(string mobileNumber, string homeNumber)
        {
            string phoneNumber = null;

            if (!string.IsNullOrEmpty(mobileNumber))
            {
                phoneNumber = mobileNumber;
            }
            else if (!string.IsNullOrEmpty(homeNumber))
            {
                phoneNumber = homeNumber;
            }

            return phoneNumber;
        }

        private string GetStateString(State state)
        {
            if (state == State.Unknown)
            {
                return null;
            }

            return state.ToFriendlyString();
        }
    }
}
using System;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface ISearchQuotesClientsAndProspectsRequestConverter
    {
        SearchQuotesClientsAndProspectsRequest From(SearchClientsRequest searchClientsRequest);
    }

    public class SearchQuotesClientsAndProspectsRequestConverter : ISearchQuotesClientsAndProspectsRequestConverter
    {
        public SearchQuotesClientsAndProspectsRequest From(SearchClientsRequest searchClientsRequest)
        {
            var searchType = GetSearchType(searchClientsRequest);

            if (searchType == SearchType.LeadId)
            {
                long? leadId;
                TryParseNullableLong(searchClientsRequest.LeadId, out leadId);

                if (!leadId.HasValue)
                {
                    throw new ApplicationException("Cannot create a leadId Search request without a lead id");
                }

                return SearchQuotesClientsAndProspectsRequest.LeadIdSearch(leadId.Value);
            }

            if (searchType == SearchType.QuoteReference)
            {
                return SearchQuotesClientsAndProspectsRequest.QuoteReferenceSearch(searchClientsRequest.QuoteReferenceNumber);
            }

            if (searchType == SearchType.PartyInformation)
            {
                return SearchQuotesClientsAndProspectsRequest.PartyInfoSearch(searchClientsRequest.FirstName,
                    searchClientsRequest.Surname,
                    searchClientsRequest.DateOfBirth.ToDateExcactDdMmYyyy(),
                    searchClientsRequest.MobileNumber,
                    searchClientsRequest.HomeNumber,
                    searchClientsRequest.EmailAddress,
                    searchClientsRequest.ExternalCustomerReference);
            }

            throw new ApplicationException("No valid Search Type supplied");
        }

        private SearchType GetSearchType(SearchClientsRequest searchClientsRequest)
        {
            if(searchClientsRequest.SearchOnQuoteReference)
                return SearchType.QuoteReference;
            if(searchClientsRequest.SearchOnLeadId)
                return SearchType.LeadId;
            if (searchClientsRequest.SearchOnParty)
                return SearchType.PartyInformation;

            throw new ApplicationException("There are no appropriate search types for this search request.");
        }

        public static bool TryParseNullableLong(string text, out long? outValue)
        {
            long parsedValue;
            bool success = long.TryParse(text, out parsedValue);
            outValue = success ? (long?)parsedValue : null;
            return success;
        }
    }
}
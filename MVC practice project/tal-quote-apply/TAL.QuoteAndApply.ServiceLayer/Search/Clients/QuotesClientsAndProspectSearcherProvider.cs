using System;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients
{
    public interface IQuotesClientsAndProspectSearcherProvider
    {
        IQuotesClientsAndProspectSearcher GetFor(
            SearchQuotesClientsAndProspectsRequest searchRequest);
    }

    public class QuotesClientsAndProspectSearcherProvider : IQuotesClientsAndProspectSearcherProvider
    {
        private readonly ISearchByQuoteReferenceNumberService _searchByQuoteReferenceNumberService;
        private readonly ISearchByLeadIdService _searchByLeadIdService;
        private readonly ISearchByPartyInformationService _searchByPartyInformationService;

        public QuotesClientsAndProspectSearcherProvider(ISearchByQuoteReferenceNumberService searchByQuoteReferenceNumberService, 
            ISearchByLeadIdService searchByLeadIdService, ISearchByPartyInformationService searchByPartyInformationService)
        {
            _searchByQuoteReferenceNumberService = searchByQuoteReferenceNumberService;
            _searchByLeadIdService = searchByLeadIdService;
            _searchByPartyInformationService = searchByPartyInformationService;
        }

        public IQuotesClientsAndProspectSearcher GetFor(
            SearchQuotesClientsAndProspectsRequest searchRequest)
        {
            if (searchRequest.SearchType == SearchType.QuoteReference)
            {
                return _searchByQuoteReferenceNumberService;
            }

            if (searchRequest.SearchType == SearchType.LeadId)
            {
                return _searchByLeadIdService;
            }

            if (searchRequest.SearchType == SearchType.PartyInformation)
            {
                return _searchByPartyInformationService;
            }

            throw new ApplicationException("Could not find a searcher for this search request");
        }
    }
}
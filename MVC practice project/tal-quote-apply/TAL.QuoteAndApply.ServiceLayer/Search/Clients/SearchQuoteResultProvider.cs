using System;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Leads;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Clients
{
    public interface ISearchQuoteResultProvider
    {
        SearchResult From(IPolicySearchResult policySearchResult);
        SearchResult From(GetLeadResult getLeadResult);
    }

    public class SearchQuoteResultProvider : ISearchQuoteResultProvider
    {
        private readonly IProductBrandProvider _brandProvider;

        public SearchQuoteResultProvider(IProductBrandProvider brandProvider)
        {
            _brandProvider = brandProvider;
        }

        public SearchResult From(IPolicySearchResult policySearchResult)
        {
            return new SearchResult(policySearchResult.QuoteReference, policySearchResult.Premium, policySearchResult.LeadId, policySearchResult.FirstName, policySearchResult.Surname, 
                policySearchResult.DateOfBirth, policySearchResult.State, policySearchResult.MobileNumber, policySearchResult.HomeNumber, policySearchResult.EmailAddress, "", policySearchResult.ExternalCustomerReference, _brandProvider.GetBrandKeyByBrandId(policySearchResult.BrandId));
        }

        public SearchResult From(GetLeadResult getLeadResult)
        {
            return new SearchResult(null, 0.0m, long.Parse(getLeadResult.AdobeId), getLeadResult.FirstName, getLeadResult.Surname, getLeadResult.DateOfBirth, 
                (State)Enum.Parse(typeof(State), getLeadResult.State ?? "Unknown", true), getLeadResult.MobileNumber, getLeadResult.HomeNumber, getLeadResult.EmailAddress, getLeadResult.Gender, getLeadResult.ExternalCustomerReference, getLeadResult.Brand);
        }
    }
}
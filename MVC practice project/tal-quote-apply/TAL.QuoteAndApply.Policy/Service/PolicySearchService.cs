using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{

    public interface IPolicySearchService
    {
        IEnumerable<IPolicySearchResult> SearchByQuoteReference(string quoteReference);
        IEnumerable<IPolicySearchResult> SearchByLeadId(long leadId);
        IEnumerable<IPolicySearchResult> SearchByPolicyOwnerDetails(string firstName, string surname,
            DateTime? dateOfBirth, string mobileNumber, string homeNumber, string email, string externalCustomerReference, int? brandId);
    }

    public class PolicySearchService : IPolicySearchService
    {
        private readonly IPolicySearchDtoRepository _policySearchDtoRepository;

        public PolicySearchService(IPolicySearchDtoRepository policySearchDtoRepository)
        {
            _policySearchDtoRepository = policySearchDtoRepository;
        }

        public IEnumerable<IPolicySearchResult> SearchByQuoteReference(string quoteReference)
        {
            return _policySearchDtoRepository.SearchByQuoteReference(quoteReference);
        }

        public IEnumerable<IPolicySearchResult> SearchByLeadId(long leadId)
        {
            return _policySearchDtoRepository.SearchByLeadId(leadId);
        }

        public IEnumerable<IPolicySearchResult> SearchByPolicyOwnerDetails(string firstName, string surname,
            DateTime? dateOfBirth, string mobileNumber, string homeNumber, string email, string externalCustomerReference= null, int? brandId = null)
        {
            return _policySearchDtoRepository.SearchByPolicyOwnerDetails(firstName, surname, dateOfBirth, mobileNumber,
                homeNumber, email, externalCustomerReference, brandId);
        }
    }
}

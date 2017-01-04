using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IPolicySearchDtoRepository
    {
        IEnumerable<PolicySearchResultDto> SearchByQuoteReference(string quoteReferenceNumber);
        IEnumerable<PolicySearchResultDto> SearchByLeadId(long leadId);

        IEnumerable<PolicySearchResultDto> SearchByPolicyOwnerDetails(string firstName, string surname,
            DateTime? dateOfBirth, string mobileNumber, string homeNumber, string email, string externalCustomerReference, int? brandId);
    }

    public class PolicySearchDtoRepository : BaseRepository<PolicySearchResultDto>, IPolicySearchDtoRepository
    {
        public PolicySearchDtoRepository(IPolicyConfigurationProvider policyConfigurationProvider, ICurrentUserProvider currentUserProvider, IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService) 
            : base(policyConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public IEnumerable<PolicySearchResultDto> SearchByQuoteReference(string quoteReferenceNumber)
        {
            return Where(policySearch => policySearch.QuoteReference, Op.Eq, quoteReferenceNumber);
        }

        public IEnumerable<PolicySearchResultDto> SearchByLeadId(long leadId)
        {
            return Where(policySearch => policySearch.LeadId, Op.Eq, leadId);
        }

        public IEnumerable<PolicySearchResultDto> SearchByPolicyOwnerDetails(string firstName, string surname,
            DateTime? dateOfBirth, string mobileNumber, string homeNumber, string email, string externalCustomerReference = null, int? brandId = null)
        {
            var q = DbItemPredicate<PolicySearchResultDto>
                .Empty();

            if (!string.IsNullOrEmpty(firstName))
            {
                q.And(policySearch => policySearch.FirstName, Op.StartsWith, firstName);
            }

            if (!string.IsNullOrEmpty(surname))
            {
                q.And(policySearch => policySearch.Surname, Op.StartsWith, surname);
            }

            if (!string.IsNullOrEmpty(email))
            {
                q.And(policySearch => policySearch.EmailAddress, Op.Eq, email);
            }

            if (!string.IsNullOrEmpty(mobileNumber))
            {
                q.And(policySearch => policySearch.MobileNumber, Op.Eq, mobileNumber);
            }

            if (!string.IsNullOrEmpty(homeNumber))
            {
                q.And(policySearch => policySearch.HomeNumber, Op.Eq, homeNumber);
            }

            if (dateOfBirth != null)
            {
                q.And(policySearch => policySearch.DateOfBirth, Op.Eq, dateOfBirth);
            }

            if (!string.IsNullOrEmpty(externalCustomerReference))
            {
                q.And(policySearch => policySearch.ExternalCustomerReference, Op.Eq, externalCustomerReference);
            }

            if (brandId != null)
            {
                q.And(policySearch => policySearch.BrandId, Op.Eq, brandId);
            }
            return Query(q, 20); // TODO: configure this magic number
        }
    }
}

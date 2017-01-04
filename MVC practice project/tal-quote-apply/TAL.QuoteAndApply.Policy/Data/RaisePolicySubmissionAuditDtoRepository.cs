using System.Collections.Generic;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IRaisePolicySubmissionAuditDtoRepository
    {
        RaisePolicySubmissionAuditDto InsertRaisePolicySubmissionAudit(RaisePolicySubmissionAuditDto raisePolicySubmissionAudit);
        IEnumerable<RaisePolicySubmissionAuditDto> GetAllAuditsForPolicy(int policyId);
    }

    public class RaisePolicySubmissionAuditDtoRepository : BaseRepository<RaisePolicySubmissionAuditDto>, IRaisePolicySubmissionAuditDtoRepository
    {
        public RaisePolicySubmissionAuditDtoRepository(IPolicyConfigurationProvider policyConfigurationProvider, ICurrentUserProvider currentUserProvider, 
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService) 
            : base(policyConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public RaisePolicySubmissionAuditDto InsertRaisePolicySubmissionAudit(RaisePolicySubmissionAuditDto raisePolicySubmissionAudit)
        {
            return Insert(raisePolicySubmissionAudit);
        }

        public IEnumerable<RaisePolicySubmissionAuditDto> GetAllAuditsForPolicy(int policyId)
        {
            return Where(p => p.PolicyId, Op.Eq, policyId);
        }
    }
}

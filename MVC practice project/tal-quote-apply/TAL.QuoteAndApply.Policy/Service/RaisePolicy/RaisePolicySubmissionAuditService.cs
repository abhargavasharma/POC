using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service.RaisePolicy
{
    public interface IRaisePolicySubmissionAuditService
    {
        void WriteSubmissionAudit(int policyId, PolicyNewBusinessOrderProcess_Type newBusinessOrderProcessType);
        void WriteFeedbackAudit(int policyId, string rawXml);
    }

    public class RaisePolicySubmissionAuditService : IRaisePolicySubmissionAuditService
    {
        private readonly IRaisePolicySubmissionAuditDtoRepository _raisePolicySubmissionAuditDtoRepository;

        public RaisePolicySubmissionAuditService(IRaisePolicySubmissionAuditDtoRepository raisePolicySubmissionAuditDtoRepository)
        {
            _raisePolicySubmissionAuditDtoRepository = raisePolicySubmissionAuditDtoRepository;
        }

        public void WriteSubmissionAudit(int policyId, PolicyNewBusinessOrderProcess_Type newBusinessOrderProcessType)
        {
            _raisePolicySubmissionAuditDtoRepository.InsertRaisePolicySubmissionAudit(
                new RaisePolicySubmissionAuditDto
                {
                    PolicyId = policyId,
                    RaisePolicyXml = newBusinessOrderProcessType.ToXml(),
                    RaisePolicyAuditType = RaisePolicyAuditType.Submit
                });
        }

        public void WriteFeedbackAudit(int policyId, string rawXml)
        {
            _raisePolicySubmissionAuditDtoRepository.InsertRaisePolicySubmissionAudit(
                new RaisePolicySubmissionAuditDto
                {
                    PolicyId = policyId,
                    RaisePolicyXml = rawXml,
                    RaisePolicyAuditType = RaisePolicyAuditType.Feedback
                });
        }
    }
}

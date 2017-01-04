using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class RaisePolicySubmissionAuditDtoMapper : DbItemClassMapper<RaisePolicySubmissionAuditDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<RaisePolicySubmissionAuditDto> mapper)
        {
            mapper.MapTable("RaisePolicySubmissionAudit");
            mapper.MapProperty(audit => audit.RaisePolicyAuditType, "RaisePolicyAuditTypeId");
        }
    }
}
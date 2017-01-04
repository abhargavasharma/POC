using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class PolicyAnalyticsIdentifierDtoClassMapper : DbItemClassMapper<PolicyAnalyticsIdentifierDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PolicyAnalyticsIdentifierDto> mapper)
        {
            mapper.MapTable("PolicyAnalyticsIdentifier");
        }
    }
}
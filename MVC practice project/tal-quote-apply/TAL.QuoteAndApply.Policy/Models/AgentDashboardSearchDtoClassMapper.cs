
using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class AgentDashboardSearchDtoClassMapper : DbItemClassMapper<AgentDashboardSearchDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<AgentDashboardSearchDto> mapper)
        {
            mapper.MapTable("vwAgentDashboardSearch");
            mapper.MapProperty(policySearch => policySearch.Id, "PolicyId");
        }
    }
}
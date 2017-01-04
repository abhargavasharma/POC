using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class PolicySearchDtoClassMapper : DbItemClassMapper<PolicySearchResultDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PolicySearchResultDto> mapper)
        {
            mapper.MapTable("vwPolicySearch");
            mapper.MapProperty(policySearch => policySearch.Id, "PolicyId");
            mapper.MapProperty(policySearch => policySearch.State, "StateId");
        }
    }
}
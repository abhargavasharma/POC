using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class PolicyOwnerDtoClassMapper : DbItemClassMapper<PolicyOwnerDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PolicyOwnerDto> mapper)
        {
            mapper.MapTable("PolicyOwner");

            mapper.MapProperty(p => p.OwnerType, "PolicyOwnerTypeId");
        }
    }
}
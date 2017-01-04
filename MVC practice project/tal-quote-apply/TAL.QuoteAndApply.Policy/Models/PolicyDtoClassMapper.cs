using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class PolicyDtoClassMapper : DbItemClassMapper<PolicyDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PolicyDto> mapper)
        {
            mapper.MapTable("Policy");
            mapper.MapProperty(p => p.Status, "PolicyStatusId");
            mapper.MapProperty(p => p.PremiumFrequency, "PremiumFrequencyId");
            mapper.MapProperty(p => p.Progress, "PolicyProgressId");
            mapper.MapProperty(p => p.Source, "PolicySourceId");

            mapper.Ignore(p => p.BrandKey);
        }
    }
}
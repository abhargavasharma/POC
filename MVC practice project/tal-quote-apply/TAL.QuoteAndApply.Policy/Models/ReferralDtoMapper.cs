using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class ReferralDtoMapper : DbItemClassMapper<ReferralDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<ReferralDto> mapper)
        {
            mapper.MapTable("PolicyReferral");
        }
    }
}
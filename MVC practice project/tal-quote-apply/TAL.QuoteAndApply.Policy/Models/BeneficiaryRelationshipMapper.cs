using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class BeneficiaryRelationshipMapper : DbItemClassMapper<BeneficiaryRelationshipDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<BeneficiaryRelationshipDto> mapper)
        {
            mapper.MapTable("BeneficiaryRelationship");
        }
    }
}

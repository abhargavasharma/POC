using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class OrganisationDtoClassMapper : DbItemClassMapper<OrganisationDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<OrganisationDto> mapper)
        {
            mapper.MapTable("Organisation");
        }
    }
}

using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.Policy.Data;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class OptionDtoClassMapper : DbItemClassMapper<OptionDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<OptionDto> mapper)
        {
            mapper.MapTable("Option");
        }
    }
}
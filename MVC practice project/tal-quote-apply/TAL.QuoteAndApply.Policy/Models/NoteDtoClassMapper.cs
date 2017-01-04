using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class NoteDtoClassMapper : DbItemClassMapper<NoteDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<NoteDto> mapper)
        {
            mapper.MapTable("Note");
        }
    }
}
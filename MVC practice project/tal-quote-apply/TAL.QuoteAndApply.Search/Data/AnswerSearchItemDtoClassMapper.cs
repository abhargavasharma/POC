using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Search.Data
{
    public class AnswerSearchItemDtoClassMapper : DbItemClassMapper<AnswerSearchItemDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<AnswerSearchItemDto> mapper)
        {
            mapper.MapTable("AnswerSearchItem");
        }
    }
}
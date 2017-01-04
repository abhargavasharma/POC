using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Search.Data
{
    public class AnswerSearchItemDto : DbItem
    {
        public string IndexKey { get; set; }
        public string ResponseId { get; set; }
        public string Text { get; set; }
        public string HelpText { get; set; }
        public string Tags { get; set; }
        public string ParentId { get; set; }
    }
}

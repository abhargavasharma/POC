using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Search.LuceneWrapper;
using TAL.QuoteAndApply.Search.Models;

namespace TAL.QuoteAndApply.Search.Service
{
    public interface IQuestionAnswerWriter<T>
    {
        void AddUpdateSampleDataToIndex(List<QuestionAnswerData<T>> items, List<T> rawData );
    }

    public class QuestionAnswerWriter : BaseWriter<AnswerSearchItemDto>, IQuestionAnswerWriter<AnswerSearchItemDto>
    {
        private readonly string _key;

        public IAnswerSearchItemRepository Repository { get; private set; }


        public QuestionAnswerWriter(LuceneParameters parameters, IAnswerSearchItemRepository repo) : base(parameters)
        {
            Repository = repo;
            _key = parameters.IndexName;
        }

        public void AddUpdateSampleDataToIndex(List<QuestionAnswerData<AnswerSearchItemDto>> items, List<AnswerSearchItemDto> rawData )
        {
            Repository.SaveAll(rawData, _key);
            AddUpdateItemsToIndex(items.Select(Map).ToList());
        }

        private QuestionAnswerDocument Map(QuestionAnswerData<AnswerSearchItemDto> item)
        {
            return new QuestionAnswerDocument()
            {
                Name = item.Value,
                Similies = item.Similies,
                Id = item.Id
            };
        }
    }
}

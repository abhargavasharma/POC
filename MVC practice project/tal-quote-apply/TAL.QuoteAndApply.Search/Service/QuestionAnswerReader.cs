using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Search.LuceneWrapper;

namespace TAL.QuoteAndApply.Search.Service
{
    public interface IQuestionAnswerReader<T>
    {
        List<T> RawAnswerData { get; }
        int NumberOfIndexedDocuments { get; }
    }

    public class QuestionAnswerReader : BaseReader<AnswerSearchItemDto>, IQuestionAnswerReader<AnswerSearchItemDto>
    {
        private readonly string _key;

        public IAnswerSearchItemRepository Repository { get; private set; }


        public 
            QuestionAnswerReader(LuceneParameters parameters, IAnswerSearchItemRepository repo) : base(parameters)
        {
            Repository = repo;
            _key = parameters.IndexName;
        }

        public List<AnswerSearchItemDto> RawAnswerData => Repository.GetAll(_key).ToList();

        private int? _numberOfDocuments;
        public int NumberOfIndexedDocuments
        {
            get
            {
                if (!_numberOfDocuments.HasValue)
                {
                    _numberOfDocuments = GetNumberOfDocuments();
                }
                return _numberOfDocuments.Value;
            }
        }
    }
}
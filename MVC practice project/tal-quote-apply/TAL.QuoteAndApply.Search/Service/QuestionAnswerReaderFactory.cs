using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Search.LuceneWrapper;

namespace TAL.QuoteAndApply.Search.Service
{
    public interface IQuestionAnswerReaderFactory
    {
        IQuestionAnswerReader<AnswerSearchItemDto> Create(LuceneParameters parameters);
    }

    public class QuestionAnswerReaderFactory : IQuestionAnswerReaderFactory
    {
        private readonly IAnswerSearchItemRepository _answerSearchItemRepository;

        public QuestionAnswerReaderFactory(IAnswerSearchItemRepository answerSearchItemRepository)
        {
            _answerSearchItemRepository = answerSearchItemRepository;
        }

        public IQuestionAnswerReader<AnswerSearchItemDto> Create(LuceneParameters parameters)
        {
            return new QuestionAnswerReader(parameters, _answerSearchItemRepository);
        }
    }
}

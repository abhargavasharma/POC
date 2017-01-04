using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Search.LuceneWrapper;

namespace TAL.QuoteAndApply.Search.Service
{
    public interface IQuestionAnswerWriterFactory
    {
        IQuestionAnswerWriter<AnswerSearchItemDto> Create(LuceneParameters parameters);
    }

    public class QuestionAnswerWriterFactory : IQuestionAnswerWriterFactory
    {
        private readonly IAnswerSearchItemRepository _answerSearchItemRepository;

        public QuestionAnswerWriterFactory(IAnswerSearchItemRepository answerSearchItemRepository)
        {
            _answerSearchItemRepository = answerSearchItemRepository;
        }

        public IQuestionAnswerWriter<AnswerSearchItemDto> Create(LuceneParameters parameters)
        {
            return new QuestionAnswerWriter(parameters, _answerSearchItemRepository);
        }
    }
}

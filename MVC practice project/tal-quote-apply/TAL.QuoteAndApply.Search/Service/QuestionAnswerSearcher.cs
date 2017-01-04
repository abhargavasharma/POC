using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Search.LuceneWrapper;
using TAL.QuoteAndApply.Search.Models;

namespace TAL.QuoteAndApply.Search.Service
{
    public interface IQuestionAnswerSearcher<T>
    {
        List<T> RawSearchData { get; }
        List<QuestionAnswerData<T>> SearchData { get; }
        SearchResult SearchAnswers(string searchTerm);
    }

    public class QuestionAnswerSearcher<T>: BaseSearcher<T>, IQuestionAnswerSearcher<T>
    {
        public List<T> RawSearchData { get; private set; }
        public List<QuestionAnswerData<T>> SearchData { get; private set; }

        public QuestionAnswerSearcher(LuceneParameters luceneParameters, IEnumerable<QuestionAnswerData<T>> searchData, IEnumerable<T> rawDataSource)
            : base(luceneParameters)
        {
            SearchData = searchData.ToList();
            RawSearchData = rawDataSource.ToList();
        }

        public SearchResult SearchAnswers(string searchTerm)
        {
            return Search(searchTerm.ToLower());
        }
    }
}

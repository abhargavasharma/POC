using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Search.Configuration;
using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Search.LuceneWrapper;
using TAL.QuoteAndApply.Search.Models;
using TAL.QuoteAndApply.Search.Service;
using TAL.QuoteAndApply.ServiceLayer.Search.Models;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Underwriting
{
    public class DrillDownAnswerSetItem
    {
        public DrillDownAnswerSetItem(IEnumerable<ReadOnlyAnswer> answers, ReadOnlyAnswer paretAnswer)
        {
            Answers = answers;
            ParetAnswer = paretAnswer;
        }

        public IEnumerable<ReadOnlyAnswer> Answers { get; private set; }
        public ReadOnlyAnswer ParetAnswer { get; private set; }
    }

    public interface IQuestionAnswerSearcherProvider
    {
        void EnsureIndex(IEnumerable<ReadOnlyAnswer> answers, string identifyingKey, string templateVersion);
        void EnsureIndex(IEnumerable<DrillDownAnswerSetItem> answers, string identifyingKey, string templateVersion);
        IQuestionAnswerSearcher<AnswerSearchItemDto> GetSearcher(string hashedIdentityKey, string templateVersion);
    }
    public class QuestionAnswerSearcherProvider : IQuestionAnswerSearcherProvider
    {
        private static readonly object PadLock;
        private static readonly IDictionary<string, IQuestionAnswerSearcher<AnswerSearchItemDto>> Searchers;

        private readonly ISearchConfigurationProvider _searchConfigurationProvider;
        private readonly IQuestionAnswerReaderFactory _questionAnswerReaderFactory;
        private readonly IQuestionAnswerWriterFactory _questionAnswerWriterFactory;
        private readonly IQuestionAnswerDataProvider _questionAnswerDataProvider;

        static QuestionAnswerSearcherProvider()
        {
            PadLock = new object();
            Searchers = new Dictionary<string, IQuestionAnswerSearcher<AnswerSearchItemDto>>();
        }

        public QuestionAnswerSearcherProvider(ISearchConfigurationProvider searchConfigurationProvider, IQuestionAnswerReaderFactory questionAnswerReaderFactory, IQuestionAnswerWriterFactory questionAnswerWriterFactory, IQuestionAnswerDataProvider questionAnswerDataProvider)
        {
            _searchConfigurationProvider = searchConfigurationProvider;
            _questionAnswerReaderFactory = questionAnswerReaderFactory;
            _questionAnswerWriterFactory = questionAnswerWriterFactory;
            _questionAnswerDataProvider = questionAnswerDataProvider;
        }

        public void EnsureIndex(IEnumerable<ReadOnlyAnswer> answers, string identifyingKey, string templateVersion)
        {
            var hashedQuestionId = SearchQuestionHashProvider.CreateHashKeyFor(identifyingKey);
            GetSearcher(hashedQuestionId, templateVersion, answers);
        }

        public void EnsureIndex(IEnumerable<DrillDownAnswerSetItem> answers, string identifyingKey, string templateVersion)
        {
            var hashedQuestionId = SearchQuestionHashProvider.CreateHashKeyFor(identifyingKey);
            GetSearcher(hashedQuestionId, templateVersion, answers);
        }

        public IQuestionAnswerSearcher<AnswerSearchItemDto> GetSearcher(string hashedIdentityKey, string templateVersion)
        {
            return GetSearcher(hashedIdentityKey, templateVersion, new[] { new DrillDownAnswerSetItem(null, null) });
        }

        public IQuestionAnswerSearcher<AnswerSearchItemDto> GetSearcher(string hashedIdentityKey, string templateVersion,
            IEnumerable<ReadOnlyAnswer> answers)
        {
            return GetSearcher(hashedIdentityKey, templateVersion, new [] {new DrillDownAnswerSetItem(answers, null)});
        }

        public IQuestionAnswerSearcher<AnswerSearchItemDto> GetSearcher(string hashedIdentityKey, string templateVersion, IEnumerable<DrillDownAnswerSetItem> drillDownAnswers)
        {
            var key = GetKey(hashedIdentityKey, templateVersion);

            if (Searchers.ContainsKey(key))
                return Searchers[key]; 

            lock (PadLock)
            {
                if (Searchers.ContainsKey(key))
                    return Searchers[key];

                var luceneParams = LuceneParameters.ForSql(_searchConfigurationProvider.LuceneIndexRootPath, key,
                    _searchConfigurationProvider.ConnectionString, _searchConfigurationProvider.SearchDatabase);

                var reader = _questionAnswerReaderFactory.Create(luceneParams);
                List<QuestionAnswerData<AnswerSearchItemDto>> questionAnswerData;
                if (reader.NumberOfIndexedDocuments <= 0)
                {
                    var rawData = reader.RawAnswerData;
                    if (rawData.Count <= 0)
                    {
                        if (drillDownAnswers == null)
                        {
                            return null;
                        }

                        foreach (var drillDownAnswerSetItem in drillDownAnswers.Where(dda => dda != null))
                        {
                            if (drillDownAnswerSetItem.Answers == null)
                            {
                                continue;
                            }

                            if (drillDownAnswerSetItem.ParetAnswer == null)
                            {
                                rawData.AddRange(drillDownAnswerSetItem.Answers.Select(a => a.ToAnswerSearchItemDto()).ToList());
                            }
                            else
                            {
                                rawData.AddRange(drillDownAnswerSetItem.Answers.Select(
                                        a => a.ToAnswerSearchItemDto(drillDownAnswerSetItem.ParetAnswer)).ToList());
                                rawData.Add(drillDownAnswerSetItem.ParetAnswer.ToAnswerSearchItemDto());
                            }
                        }

                        if (rawData.Count <= 0)
                        {
                            return null;
                        }
                    }

                    var writer = _questionAnswerWriterFactory.Create(luceneParams);
                    questionAnswerData = _questionAnswerDataProvider.MapDataFrom(rawData);
                    writer.AddUpdateSampleDataToIndex(questionAnswerData, rawData);
                }
                else
                {
                    questionAnswerData = _questionAnswerDataProvider.MapDataFrom(reader.RawAnswerData);
                }

                var searcher = new QuestionAnswerSearcher<AnswerSearchItemDto>(luceneParams, questionAnswerData,
                    reader.RawAnswerData);

                Searchers.Add(key, searcher);
            }

            return Searchers[key];
        }

        private string GetKey(string hashedQuestionId, string templateVersion)
        {
            return string.Format("{0}-{1}", templateVersion, hashedQuestionId);
        }
    }
}
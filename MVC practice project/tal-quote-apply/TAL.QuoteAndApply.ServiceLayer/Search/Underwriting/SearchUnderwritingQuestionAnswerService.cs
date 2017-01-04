using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Search.Models;
using TAL.QuoteAndApply.Underwriting.Configuration;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Underwriting
{
    public interface ISearchUnderwritingQuestionAnswerService
    {
        string EnsureIndexAndGetTemplateVersion(string questionId);
        List<UnderwritingAnswerSearchResult> Search(string hashedQuestionId, string templateVersion, string query, int limit);
    }

    public class SearchUnderwritingQuestionAnswerService : ISearchUnderwritingQuestionAnswerService
    {
        private readonly IQuestionAnswerSearcherProvider _questionAnswerSearcherProvider;
        private readonly IUnderwritingTemplateService _underwritingTemplateService;
        private readonly ICreateUnderwritingInterview _createUnderwritingInterview;
        private readonly ISearchPhraseSanitizer _searchPhraseSanitizer;
        private readonly IUnderwritingConfigurationProvider _underwritingConfigurationProvider;
        
        public SearchUnderwritingQuestionAnswerService(
            IQuestionAnswerSearcherProvider questionAnswerSearcherProvider,
            IUnderwritingTemplateService underwritingTemplateService,
            ICreateUnderwritingInterview createUnderwritingInterview, 
            ISearchPhraseSanitizer searchPhraseSanitizer,
            IUnderwritingConfigurationProvider underwritingConfigurationProvider)
        {
            _questionAnswerSearcherProvider = questionAnswerSearcherProvider;
            _underwritingTemplateService = underwritingTemplateService;
            _createUnderwritingInterview = createUnderwritingInterview;
            _searchPhraseSanitizer = searchPhraseSanitizer;
            _underwritingConfigurationProvider = underwritingConfigurationProvider;
        }

        public string EnsureIndexAndGetTemplateVersion(string tag)
        {
            var templateTypeName = _underwritingConfigurationProvider.TemplateName;
            var templateInfo = _underwritingTemplateService.GetTemplateInformation(templateTypeName);
            var hashedQuestionId = SearchQuestionHashProvider.CreateHashKeyFor(tag);
            var searcher = _questionAnswerSearcherProvider.GetSearcher(hashedQuestionId, templateInfo.TemplateId);
            if (searcher == null)
            {
                var interview = _createUnderwritingInterview.CreateNewInterview(_underwritingConfigurationProvider.TemplateName, 
                    _underwritingConfigurationProvider.FullWorkflow, 
                    templateInfo.Benefits);

                var allUnasnweredQuestions = interview.Benefits.SelectMany(b => b.UnansweredQuestions).Distinct(DistinctQuestionComparer.Instance);
                var questionToIndex = allUnasnweredQuestions.First(q => q.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)));
                _questionAnswerSearcherProvider.EnsureIndex(questionToIndex.Answers, tag, interview.TemplateVersion);
            }
            return templateInfo.TemplateId;
        }
        
        public List<UnderwritingAnswerSearchResult> Search(string hashedQuestionId, string templateVersion, string query, int limit)
        {
            var searcher = _questionAnswerSearcherProvider.GetSearcher(hashedQuestionId, templateVersion);
            var santizedQuery = _searchPhraseSanitizer.SanitizePhrase(query);
            var searchResult = searcher.SearchAnswers(santizedQuery);
            var answerData = searcher.SearchData;

            var result = searchResult.SearchResultItems
                .OrderByDescending(x => x.Score)
                .Select(searchResultItem => answerData.First(answer => answer.Id == searchResultItem.Id))
                .Select(answerDataItem => UnderwritingAnswerSearchResult.From(answerDataItem.RawData))
                .Take(limit)
                .ToList();

            return result;
        }
    }
}

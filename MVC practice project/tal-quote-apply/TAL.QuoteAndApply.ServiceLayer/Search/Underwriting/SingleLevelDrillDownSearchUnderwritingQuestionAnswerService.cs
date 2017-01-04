using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Search.Models;
using TAL.QuoteAndApply.Underwriting.Configuration;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Underwriting
{
    public interface ISingleLevelDrillDownSearchUnderwritingQuestionAnswerService
    {
        string EnsureIndexForDrillDownAndGetTemplateVersion(string parentQuestionTag, string childQuestionTag);
        List<UnderwritingAnswerSearchResult> Search(string hashedIdentityKey, string templateVersion, string query, int limit);
    }

    public class SingleLevelDrillDownSearchUnderwritingQuestionAnswerService : ISingleLevelDrillDownSearchUnderwritingQuestionAnswerService
    {
        private readonly IQuestionAnswerSearcherProvider _questionAnswerSearcherProvider;
        private readonly IUnderwritingTemplateService _underwritingTemplateService;
        private readonly ICreateUnderwritingInterview _createUnderwritingInterview;
        private readonly IUpdateUnderwritingInterview _updateUnderwritingInterview;
        private readonly ISearchPhraseSanitizer _searchPhraseSanitizer;
        private readonly IUnderwritingConfigurationProvider _underwritingConfigurationProvider;

        public SingleLevelDrillDownSearchUnderwritingQuestionAnswerService(
            IQuestionAnswerSearcherProvider questionAnswerSearcherProvider,
            IUnderwritingTemplateService underwritingTemplateService,
            ICreateUnderwritingInterview createUnderwritingInterview,
            IUpdateUnderwritingInterview updateUnderwritingInterview, ISearchPhraseSanitizer searchPhraseSanitizer,
            IUnderwritingConfigurationProvider underwritingConfigurationProvider)
        {
            _questionAnswerSearcherProvider = questionAnswerSearcherProvider;
            _underwritingTemplateService = underwritingTemplateService;
            _createUnderwritingInterview = createUnderwritingInterview;
            _updateUnderwritingInterview = updateUnderwritingInterview;
            _searchPhraseSanitizer = searchPhraseSanitizer;
            _underwritingConfigurationProvider = underwritingConfigurationProvider;
        }

        public string EnsureIndexForDrillDownAndGetTemplateVersion(string parentQuestionTag, string childQuestionTag)
        {
            var templateTypeName = _underwritingConfigurationProvider.TemplateName;
            var templateInfo = _underwritingTemplateService.GetTemplateInformation(templateTypeName);
            var hashedQuestionId = SearchQuestionHashProvider.CreateHashKeyFor(parentQuestionTag);
            var searcher = _questionAnswerSearcherProvider.GetSearcher(hashedQuestionId, templateInfo.TemplateId);
            if (searcher == null)
            {
                var interview = _createUnderwritingInterview.CreateNewInterview(_underwritingConfigurationProvider.TemplateName,
                    _underwritingConfigurationProvider.FullWorkflow,
                    templateInfo.Benefits);

                var allUnasnweredQuestions = interview.Benefits.SelectMany(b => b.UnansweredQuestions).Distinct(DistinctQuestionComparer.Instance);
                var questionToIndex = allUnasnweredQuestions.First(q => q.Tags.Any(t => t.Equals(parentQuestionTag, StringComparison.OrdinalIgnoreCase)));

                var concurrencyToken = interview.ConcurrencyToken;
                var drillDownSet = new List<DrillDownAnswerSetItem>();
                foreach (var parentAnswer in questionToIndex.Answers)
                {
                    var updateResponse = _updateUnderwritingInterview.AnswerQuestion(interview.InterviewIdentifier,
                        concurrencyToken, questionToIndex.Id, new AnswerSubmission()
                        {
                            ResponseId = parentAnswer.ResponseId,
                            Text = parentAnswer.Text
                        });
                    concurrencyToken = updateResponse.ConcurrencyToken;


                    var childAnswers = updateResponse.BenefitResponses.SelectMany(br => br.AddedQuestions)
                        .FirstOrDefault(aq => aq.Tags.Any(t => t.Equals(childQuestionTag, StringComparison.OrdinalIgnoreCase)))
                        ?.Answers.ToArray() ?? new ReadOnlyAnswer[0];

                    drillDownSet.Add(new DrillDownAnswerSetItem(childAnswers, parentAnswer));

                }
                _questionAnswerSearcherProvider.EnsureIndex(drillDownSet, parentQuestionTag, interview.TemplateVersion);
            }
            return templateInfo.TemplateId;
        }

        public List<UnderwritingAnswerSearchResult> Search(string hashedIdentityKey, string templateVersion, string query, int limit)
        {
            var searcher = _questionAnswerSearcherProvider.GetSearcher(hashedIdentityKey, templateVersion);
            var santizedQuery = _searchPhraseSanitizer.SanitizePhrase(query);
            var searchResult = searcher.SearchAnswers(santizedQuery);
            var answerData = searcher.SearchData;

            var result = searchResult.SearchResultItems
                .Select(
                    searchResultItem =>
                        answerData.First(answer => answer.Id == searchResultItem.Id).WithScore(searchResultItem.Score))
                .Where(x => !string.IsNullOrEmpty(x.RawData.ParentId))
                .Select(searchResultItem =>
                    searchResultItem.Value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                        ? searchResultItem.WithScore(searchResultItem.Score*2)
                        : searchResultItem)
                .OrderByDescending(x => x.Score)
                .Select(answerDataItem =>
                    UnderwritingAnswerSearchResult.From(answerDataItem.RawData,
                        answerData.First(answer => answer.Id == answerDataItem.RawData.ParentId).RawData,
                        answerDataItem.Score))
                .Take(limit)
                .ToList();

            return result;
        }
    }

   
}
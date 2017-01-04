using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Search.Models;
using TAL.QuoteAndApply.ServiceLayer.Search.Models;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Search.Underwriting
{
    public interface IQuestionAnswerDataProvider
    {
        List<QuestionAnswerData<AnswerSearchItemDto>> MapDataFrom(ReadOnlyQuestion question);
        List<QuestionAnswerData<AnswerSearchItemDto>> MapDataFrom(IEnumerable<AnswerSearchItemDto> answers);
    }

    public class QuestionAnswerDataProvider : IQuestionAnswerDataProvider
    {
        private readonly ISearchPhraseSanitizer _searchPhraseSanitizer;

        public QuestionAnswerDataProvider(ISearchPhraseSanitizer searchPhraseSanitizer)
        {
            _searchPhraseSanitizer = searchPhraseSanitizer;
        }

        public List<QuestionAnswerData<AnswerSearchItemDto>> MapDataFrom(ReadOnlyQuestion question)
        {
            return MapDataFrom(question.Answers.Select(a => a.ToAnswerSearchItemDto()));
        }

        public List<QuestionAnswerData<AnswerSearchItemDto>> MapDataFrom(IEnumerable<AnswerSearchItemDto> answers)
        {
            string parentSimilies = "";

            var answerData = answers.Select((answerDto, idx) => new QuestionAnswerData<AnswerSearchItemDto>()
            {
                Id = answerDto.ParentId != null ? answerDto.ParentId + "|" + answerDto.ResponseId : answerDto.ResponseId,
                Value = answerDto.Text,
                Similies = BuildSimilies(answerDto.Text) + " " + BuildParentSimilies(answers, answerDto.ParentId),
                RawData = answerDto,
                AnswerId = answerDto.ResponseId
            }).ToList();
            return answerData;
        }

        public string BuildParentSimilies(IEnumerable<AnswerSearchItemDto> answers, string parentId)
        {
            if (string.IsNullOrEmpty(parentId))
            {
                return "";
            }

            var parentAnswer = answers.First(a => a.ResponseId.Equals(parentId, StringComparison.OrdinalIgnoreCase));
            return BuildSimilies(parentAnswer.Text);
        }

        public string BuildSimilies(string name)
        {
            var finalName = _searchPhraseSanitizer.SanitizePhrase(name);
            string list = "";
            string previous = "";
            foreach (var ch in finalName.ToLower().ToCharArray())
            {
                previous += ch;
                list += " " + previous;
            }
            return string.Join(" ", list.Split(' ').Distinct());
        }
    }
}

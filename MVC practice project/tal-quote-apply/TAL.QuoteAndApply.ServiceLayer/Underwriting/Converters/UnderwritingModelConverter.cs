using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Converters
{
    public interface IUnderwritingModelConverter
    {
        UnderwritingAnswerQuestionResult From(ReadOnlyUpdatedUnderwritingInterview updatedInterview);
        UnderwritingAnswerQuestionResult WithNoFilteringFrom(ReadOnlyUpdatedUnderwritingInterview updatedInterview);
        UnderwritingPosition From(ReadOnlyUnderwritingInterview interview);
        UnderwritingPosition WithNoFilteringFrom(ReadOnlyUnderwritingInterview interview);
        UnderwritingQuestion From(ReadOnlyQuestion question);
        UnderwritingChangedQuestion From(ReadOnlyChangedQuestion question);
        UnderwritingAnswer From(ReadOnlyAnswer answer);
        UnderwritingChangedAnswer From(ReadOnlyChangedAnswer answer);
    }

    public class UnderwritingModelConverter : IUnderwritingModelConverter
    {
        private readonly IUnderwritingQuestionFilterService _underwritingQuestionFilterService;

        public UnderwritingModelConverter(IUnderwritingQuestionFilterService underwritingQuestionFilterService)
        {
            _underwritingQuestionFilterService = underwritingQuestionFilterService;
        }

        public UnderwritingAnswerQuestionResult From(ReadOnlyUpdatedUnderwritingInterview updatedInterview)
        {
            var allUniqueAddedQuestionsOverAllBenefits =
                OrderQuestions(updatedInterview.BenefitResponses.SelectMany(r => r.AddedQuestions).DistinctBy(q => q.Id));

            var allUniqueRemovedQuestionIdOverAllBenefits =
                updatedInterview.BenefitResponses.SelectMany(r => r.RemovedQuestions).DistinctBy(q => q.Id);

            var allUniqueChangedQuestionsOverAllBenefits =
                updatedInterview.BenefitResponses.SelectMany(r => r.ChangedQuestions).DistinctBy(q => q.Id);

            return new UnderwritingAnswerQuestionResult(
                GetFilteredQuestions(allUniqueAddedQuestionsOverAllBenefits).Select(From),
                allUniqueRemovedQuestionIdOverAllBenefits.Select(q => q.Id), //TODO: should these be run through filter?
                GetFilteredQuestions(allUniqueChangedQuestionsOverAllBenefits).Select(From)
                );
        }

        public UnderwritingAnswerQuestionResult WithNoFilteringFrom(ReadOnlyUpdatedUnderwritingInterview updatedInterview)
        {
            var allUniqueAddedQuestionsOverAllBenefits =
                OrderQuestions(updatedInterview.BenefitResponses.SelectMany(r => r.AddedQuestions).DistinctBy(q => q.Id));

            var allUniqueRemovedQuestionIdOverAllBenefits =
                updatedInterview.BenefitResponses.SelectMany(r => r.RemovedQuestions).DistinctBy(q => q.Id);

            var allUniqueChangedQuestionsOverAllBenefits =
                updatedInterview.BenefitResponses.SelectMany(r => r.ChangedQuestions).DistinctBy(q => q.Id);

            return new UnderwritingAnswerQuestionResult(
                allUniqueAddedQuestionsOverAllBenefits.Select(WithNoFilteringFrom),
                allUniqueRemovedQuestionIdOverAllBenefits.Select(q => q.Id), //TODO: should these be run through filter?
                allUniqueChangedQuestionsOverAllBenefits.Select(From)
                );
        }

        public UnderwritingPosition From(ReadOnlyUnderwritingInterview interview)
        {
            var allQuestions = interview.Benefits.SelectMany(b => b.AnsweredQuestions)
                .Concat(interview.Benefits.SelectMany(b => b.UnansweredQuestions))
                .DistinctBy(q => q.Id);

            var allFilteredQuestions = GetFilteredQuestions(allQuestions);
            var categories = interview.Benefits.SelectMany(b => b.Categories).DistinctBy(c=> c.Id);

            var orderedQuestions = QuestionSortingService.Sort(categories, allFilteredQuestions);

            return new UnderwritingPosition(interview.ConcurrencyToken, categories.Select(From), orderedQuestions.Select(From));
        }

        public UnderwritingPosition WithNoFilteringFrom(ReadOnlyUnderwritingInterview interview)
        {
            var allQuestions = interview.Benefits.SelectMany(b => b.AnsweredQuestions)
                .Concat(interview.Benefits.SelectMany(b => b.UnansweredQuestions))
                .DistinctBy(q => q.Id);
            
            var categories = interview.Benefits.SelectMany(b => b.Categories).DistinctBy(c => c.Id);

            var orderedQuestions = QuestionSortingService.Sort(categories, allQuestions);

            return new UnderwritingPosition(interview.ConcurrencyToken, categories.Select(From), orderedQuestions.Select(WithNoFilteringFrom));
        }

        /// <summary>
        /// Order questions from TALUS based on OrderId. Also takes into consideration is ParentID
        /// </summary>
        /// <param name="questions">List of questions to order</param>
        /// <returns>Ordered questions</returns>
        private IList<ReadOnlyQuestion> OrderQuestions(IEnumerable<ReadOnlyQuestion> questions)
        {
            var processedQuestions = new List<ReadOnlyQuestion>();
            var unprocessedQuestions = questions.ToList();

            foreach (var question in unprocessedQuestions)
            {
                //skip processed questions 
                if(processedQuestions.Contains(question))
                    continue;
                
                //find questions that share the same parent ID
                var orderedQuestions = unprocessedQuestions
                    .Where(q => q.ParentId == question.ParentId)
                    .OrderBy(q=>q.OrderId);

                processedQuestions.AddRange(orderedQuestions);
            }

            return processedQuestions;
        }

        private IEnumerable<ReadOnlyQuestion> GetFilteredQuestions(IEnumerable<ReadOnlyQuestion> questions)
        {
            return questions.Where(q => !_underwritingQuestionFilterService.IsHidden(q));
        }

        private IEnumerable<ReadOnlyChangedQuestion> GetFilteredQuestions(IEnumerable<ReadOnlyChangedQuestion> questions)
        {
            return questions.Where(q => !_underwritingQuestionFilterService.IsHidden(q));
        }

        private IEnumerable<ReadOnlyAnswer> GetFilteredAnswers(IEnumerable<ReadOnlyAnswer> answers)
        {
            return answers.Where(a => !_underwritingQuestionFilterService.IsHidden(a));
        }

        public UnderwritingQuestion From(ReadOnlyQuestion question)
        {
            var filteredAnswers = GetFilteredAnswers(question.Answers).ToList();
            var questionType = MapQuestionType(question, filteredAnswers);
            
            return new UnderwritingQuestion(
                questionType,
                question.Id,
                question.ParentId,
                question.Statement,
                filteredAnswers.OrderBy(a=>a.OrderId).Select(From).ToList(),
                question.Tags, 
                question.HelpText,
                question.Category,
                question.OrderId);
        }

        public UnderwritingQuestion WithNoFilteringFrom(ReadOnlyQuestion question)
        {
            var questionType = MapQuestionType(question, question.Answers.ToList());

            return new UnderwritingQuestion(
                questionType,
                question.Id,
                question.ParentId,
                question.Statement,
                question.Answers.OrderBy(a => a.OrderId).Select(From).ToList(),
                question.Tags,
                question.HelpText,
                question.Category,
                question.OrderId);
        }

        public UnderwritingChangedQuestion From(ReadOnlyChangedQuestion question)
        {
            return new UnderwritingChangedQuestion(
                question.Id,
                question.ParentId,
                question.Statement?.Value,
                question.ChangedAnswers.Value.Select(From),
                question.HelpText?.Value);
        }

        public UnderwritingAnswer From(ReadOnlyAnswer answer)
        {
            return new UnderwritingAnswer(
                answer.ResponseId,
                answer.Text == "Freetext"? answer.SelectedText: answer.Text,    //todo: confirm that this is the best way to check a free-text
                answer.SelectedId,
                answer.Selected,
                answer.Tags,
                MapAnswerType(answer),
                answer.HelpText,
                answer.Loadings?.Select(From),
                answer.Exclusions?.Select(From));
        }

        public UnderwritingCategory From(ReadOnlyCategory category)
        {
            return new UnderwritingCategory(
                category.Id,
                category.Name,
                category.OrderId);
        }

        public UnderwritingChangedAnswer From(ReadOnlyChangedAnswer answer)
        {
            return new UnderwritingChangedAnswer(answer.ResponseId, answer.SelectedId, answer.Selected);
        }

        private UnderwritingLoading From(ReadOnlyLoading loading)
        {
            return new UnderwritingLoading(loading.Name, loading.LoadingType, loading.Amount);
        }

        private UnderwritingExclusion From(ReadOnlyExclusion exclusion)
        {
            return new UnderwritingExclusion(exclusion.Name, exclusion.Text);
        }

        private UnderwritingAnswerType MapAnswerType(ReadOnlyAnswer answer)
        {
            //TODO: use answer tags instead of text
            switch (answer.Text.ToLower())
            {
                case "none": 
                case "none of the above":
                    return UnderwritingAnswerType.None;

                case "other": 
                case "more than one of the above":
                    return UnderwritingAnswerType.Other;

                case "don't know": 
                case "i don't know":
                    return UnderwritingAnswerType.IdontKnow;

                default:
                    return UnderwritingAnswerType.Default;
            }
        }
        
        private UnderwritingQuestionType MapQuestionType(ReadOnlyQuestion question, IList<ReadOnlyAnswer> filteredAnswers)
        {
            
            //TODO: sort out proper mappings once we know more about questions/control types
            switch (question.ControlType)
            {
                case ControlType.Option:
                case ControlType.SingleSelectList:
                    return MapSingleSelectQuestionType(question.Id, question.Tags, filteredAnswers.Count);

                case ControlType.MultipleChoice:
                case ControlType.MultiSelectSearchList:
                    return MapMultiSelectQuestionType(filteredAnswers.Count, question.Tags);

                case ControlType.Currency:
                    return UnderwritingQuestionType.Currency;

                case ControlType.DateAge:
                    return UnderwritingQuestionType.DateOfBirth;
                    
                default:
                    //TODO: probably throw exception once we know all control types we are supporting?
                    //throw new InvalidEnumArgumentException("No mapping for Talus ControlType");
                    return UnderwritingQuestionType.Unsupported;
            }
        }
        
        private UnderwritingQuestionType MapSingleSelectQuestionType(string questionId, IReadOnlyList<string> questionTags, int answersCount)
        {
            if (questionTags.Contains("DIGITAL_HEIGHT")
                || questionTags.Contains("DIGITAL_WEIGHT"))
                return UnderwritingQuestionType.SingleSelectAutoComplete;

            if (questionTags.Contains("GLOBAL_OCCUPATION_QUESTION")
                || questionTags.Contains("GLOBAL_INDUSTRY_QUESTION"))
                return UnderwritingQuestionType.SingleSelectAutoComplete;

            return answersCount < 12 ?    //US18765
                        UnderwritingQuestionType.SingleSelect :
                        UnderwritingQuestionType.SingleSelectAutoComplete;
        }

        private UnderwritingQuestionType MapMultiSelectQuestionType(int answersCount, IReadOnlyList<string> questionTags)
        {
            if (questionTags.Contains("SPORT_ICON_QUESTION"))
            {
                return UnderwritingQuestionType.SportIcons;
            }

            return answersCount < 12 ?    //US18767
                        UnderwritingQuestionType.MultiSelect :
                        UnderwritingQuestionType.MultiSelectAutoComplete;
        }
    }
}

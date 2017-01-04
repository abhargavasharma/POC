using System.Linq;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IUnderwritingViewModelConverter
    {
        CustomerUnderwritingResponse From(UnderwritingPosition underwritingPosition);
        AnswerQuestionResponse From(UnderwritingAnswerQuestionResult answerQuestionResult);
        QuestionResponse From(UnderwritingQuestion question);
    }

    public class UnderwritingViewModelConverter : IUnderwritingViewModelConverter
    {
        public CustomerUnderwritingResponse From(UnderwritingPosition underwritingPosition)
        {
            var answeredQuestionViewModels = underwritingPosition.Questions.Select(From).ToList();
            var categoryViewModels = underwritingPosition.Categories.Select(From).ToList();

            return new CustomerUnderwritingResponse
            {
                Status = null,
                Questions = answeredQuestionViewModels,
                Categories = categoryViewModels
            };
        }

        public AnswerQuestionResponse From(UnderwritingAnswerQuestionResult answerQuestionResult)
        {
            return new AnswerQuestionResponse(
                answerQuestionResult.AddedQuestions.Select(From),
                answerQuestionResult.RemovedQuestionIds,
                answerQuestionResult.ChangedQuestions.Select(From)
                );
        }

        public CategoryResponse From(UnderwritingCategory category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                OrderId = category.OrderId
            };
        }

        public QuestionResponse From(UnderwritingQuestion question)
        {
            return new QuestionResponse
            {
                Id = question.Id,
                ParentId = question.ParentId,
                QuestionType = question.QuestionType.ToString(),
                IsAnswered = question.IsAnswered,
                Text = question.Text,
                HelpText = question.HelpText,
                Category = question.Category,
                Answers = question.Answers.Select(From).ToList(),
                Tags = question.Tags,
                OrderId = question.OrderId
            };
        }

        public AnswerResponse From(UnderwritingAnswer answer)
        {
            return new AnswerResponse
            (
                answer.Id,
                answer.SelectedId,
                answer.IsSelected,
                answer.Text,
                answer.Tags,
                answer.AnswerType,
                answer.HelpText
            );
        }

        private static ChangedQuestionResponse From(UnderwritingChangedQuestion changedQuestion)
        {
            var changedAttributes = new ChangedAttributes
            {
                () => changedQuestion.ParentId,
                () => changedQuestion.Text
            };

            var changedQuestionResponse = new ChangedQuestionResponse(changedQuestion.Id, changedAttributes,
                changedQuestion.ChangedAnswers.Select(From), changedQuestion.HelpText);

            return changedQuestionResponse;
        }

        private static ChangedAnswerResponse From(UnderwritingChangedAnswer changedAnswer)
        {
            var changedAttributes = new ChangedAttributes
            {
                () => changedAnswer.SelectedId,
                () => changedAnswer.IsSelected
            };

            return new ChangedAnswerResponse(changedAnswer.Id, changedAttributes);
        }
    }
}
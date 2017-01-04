using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface IUnderwritingQuestionFilterService
    {
        bool IsHidden(ReadOnlyQuestion question);
        bool IsHidden(ReadOnlyChangedQuestion question);
        bool IsHidden(ReadOnlyAnswer answer);
    }

    public class UnderwritingQuestionFilterService : IUnderwritingQuestionFilterService
    {
        private readonly IList<string> _hiddenQuestionIds;

        public UnderwritingQuestionFilterService()
        {
            _hiddenQuestionIds = new List<string>
            {
                //QuestionIds.SmokerQuestion,
                //QuestionIds.DateOfBirthQuestion,
                //QuestionIds.EmploymentQuestion,
                //QuestionIds.GenderQuestion,
                //QuestionIds.AnnualIncomeQuestion 
            };
        }

        public bool IsHidden(ReadOnlyChangedQuestion question)
        {
            return IsHiddenId(question.Id) || HasExcludeTag(question.Tags.Value);
        }

        public bool IsHidden(ReadOnlyAnswer answer)
        {
            return HasExcludeTag(answer.Tags);
        }

        public bool IsHidden(ReadOnlyQuestion question)
        {
            return IsHiddenId(question.Id) || HasExcludeTag(question.Tags);
        }

        private bool IsHiddenId(string id)
        {
            return _hiddenQuestionIds.Contains(id);
        }

        private bool HasExcludeTag(IReadOnlyList<string> tags)
        {
            return tags.Contains("DIGITAL_EXCLUDE");
        }
        

    }
}

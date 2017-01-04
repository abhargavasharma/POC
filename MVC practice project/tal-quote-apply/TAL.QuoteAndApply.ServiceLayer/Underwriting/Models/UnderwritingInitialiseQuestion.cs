using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.ServiceLayer.Search.Underwriting;
using TAL.QuoteAndApply.Underwriting.Extensions;
using TAL.QuoteAndApply.Underwriting.Models.Converters;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    /*
        Helper classes for answering different types of questions.
        Maybe overkill and didn't end up working out as nicely as I thought it would in my head, but hey it works :)
    */
    public abstract class UnderwritingInitialiseQuestion
    {
        private readonly IUpdateUnderwritingInterview _updateUnderwritingInterview;
        private readonly string _interviewId;
        protected readonly ReadOnlyQuestion Question;
        
        protected UnderwritingInitialiseQuestion(string interviewId,
            ReadOnlyQuestion question, IUpdateUnderwritingInterview updateUnderwritingInterview)
        {
            Question = question;
            _updateUnderwritingInterview = updateUnderwritingInterview;
            _interviewId = interviewId;
        }

        protected abstract ReadOnlyAnswer GetAnswer();
        protected abstract string GetAnswerText();

        public ReadOnlyUpdatedUnderwritingInterview AnswerQuestion(string concurrencyToken)
        {
            var answer = GetAnswer();
            var updatedUnderwritingInterview = _updateUnderwritingInterview.AnswerQuestion(_interviewId, concurrencyToken, Question.Id,
                new AnswerSubmission
                {
                    ResponseId = answer.ResponseId,
                    Text = GetAnswerText()
                });

            return updatedUnderwritingInterview;
        }
    }

    public class UnderwritingInitialiseYesNoQuestion : UnderwritingInitialiseQuestion
    {
        private readonly bool _value;

        public UnderwritingInitialiseYesNoQuestion(bool value,
            string interviewId, ReadOnlyQuestion questions, 
            IUpdateUnderwritingInterview updateUnderwritingInterview)
            : base(interviewId, questions, updateUnderwritingInterview)
        {
            _value = value;
        }

        protected override ReadOnlyAnswer GetAnswer()
        {
            return Question.Answers.SingleOrDefaultByText(GetAnswerText());
        }

        protected override string GetAnswerText()
        {
            return _value ? "Yes" : "No";
        }
    }

    public class UnderwritingInitialiseGenderQuestion : UnderwritingInitialiseQuestion
    {
        private readonly char _gender;
        public UnderwritingInitialiseGenderQuestion(char gender, string interviewId,
            ReadOnlyQuestion questions, IUpdateUnderwritingInterview updateUnderwritingInterview)
            : base(interviewId, questions, updateUnderwritingInterview)
        {
            _gender = gender;
        }

        protected override ReadOnlyAnswer GetAnswer()
        {
            return Question.Answers.SingleOrDefaultByText(GetAnswerText());
        }

        protected override string GetAnswerText()
        {
            switch (_gender)
            {
                case 'M':
                    return "Male";
                case 'F':
                    return "Female";
                default:
                    throw new ArgumentException("Unkown gender");
            }
        }
    }

    public class UnderwritingInitialiseFreetextQuestion : UnderwritingInitialiseQuestion
    {
        private readonly string _value;

        public UnderwritingInitialiseFreetextQuestion(string value,
            string interviewId, ReadOnlyQuestion questions, IUpdateUnderwritingInterview updateUnderwritingInterview)
            : base(interviewId, questions, updateUnderwritingInterview)
        {
            _value = value;
        }

        protected override ReadOnlyAnswer GetAnswer()
        {
            return Question.Answers.First(); //Assuming freetext questions only have one answer
        }

        protected override string GetAnswerText()
        {
            return _value;
        }
    }

}

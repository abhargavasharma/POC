using System.Collections.Generic;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public enum UnderwritingQuestionChoiceType
    {
        None,
        Loading,
        Exclusion
    }

    public class UnderwritingQuestionChoiceResponse
    {
        public UnderwritingQuestionChoiceResponse(UnderwritingQuestionChoiceType currentChoiceType, string name,
            decimal premiumDiff, IList<string> applicableCoverCodes, IList<string> applicablePlanCodes, string questionId, string answerId)
        {
            CurrentChoiceType = currentChoiceType;
            Name = name;
            ApplicableCoverCodes = applicableCoverCodes;
            QuestionId = questionId;
            AnswerId = answerId;
            PremiumDiff = premiumDiff;
            ApplicablePlanCodes = applicablePlanCodes;
        }

        public UnderwritingQuestionChoiceType CurrentChoiceType { get; private set; }
        public string Name { get; private set; }
        public decimal PremiumDiff { get; private set; }
        public IList<string> ApplicableCoverCodes { get; private set; }
        public IList<string> ApplicablePlanCodes { get; private set; }
        public string QuestionId { get; private set; }
        public string AnswerId { get; private set; }
    }
}
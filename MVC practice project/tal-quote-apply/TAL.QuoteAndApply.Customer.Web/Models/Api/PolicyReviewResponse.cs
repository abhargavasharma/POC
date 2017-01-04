using System.Collections.Generic;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;
using TAL.QuoteAndApply.Customer.Web.Services;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class PolicyReviewResponse
    {
        public ReviewWorkflowStatus ReviewWorkflowStatus { get; }
        public GetPlanResponse Cover { get; }
        public IEnumerable<UnderwritingQuestionChoiceResponse> QuestionChoices { get; }
        public IEnumerable<SharedLoadingResponse> SharedLoadings { get; }

        public PolicyReviewResponse(ReviewWorkflowStatus reviewWorkflowStatus, GetPlanResponse cover,
            IEnumerable<UnderwritingQuestionChoiceResponse> questionChoices, IEnumerable<SharedLoadingResponse> sharedLoadings)
        {
            ReviewWorkflowStatus = reviewWorkflowStatus;
            Cover = cover;
            QuestionChoices = questionChoices;
            SharedLoadings = sharedLoadings;
        }
    }
}
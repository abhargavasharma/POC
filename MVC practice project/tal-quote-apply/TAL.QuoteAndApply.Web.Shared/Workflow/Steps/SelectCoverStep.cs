using System.Monads;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class SelectCoverStep : ApplicationStep
    {
        private readonly int _riskIndex;

        public SelectCoverStep(int riskIndex)
        {
            _riskIndex = riskIndex;
            CurrentStepUri.Path = "SelectCover";
        }

        public override bool ValidateStep(ApplicationStepContext context)
        {
            return context.With(ctx => ctx.Application)
                .With(app => app.Risks)
                .If(risks => risks.Count > _riskIndex)
                .Return(risks => risks[_riskIndex].InterviewStatus == InterviewStatus.Incomplete, false);
        }

        public override bool CanArriveToThisStepBackwards(ApplicationStepContext context)
        {
            return false;
        }
    }
}
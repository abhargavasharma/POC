using System.Monads;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Web.Shared.Workflow.Steps
{
    public class QualificationStep : ApplicationStep
    {
        private readonly int _riskIndex;

        public QualificationStep(int riskIndex)
        {
            _riskIndex = riskIndex;
            CurrentStepUri.Path = "Qualification";
        }

        public override bool ValidateStep(ApplicationStepContext context)
        {
            return context.With(ctx => ctx.Application)
                .With(app => app.Risks)
                .If(risks => risks.Count > _riskIndex)
                .Return(risks => risks[_riskIndex].InterviewStatus == InterviewStatus.Complete
                                 || risks[_riskIndex].InterviewStatus == InterviewStatus.Referred, false);
        }

        public override bool CanArriveToThisStepBackwards(ApplicationStepContext context)
        {
            return false;
        }
    }
}
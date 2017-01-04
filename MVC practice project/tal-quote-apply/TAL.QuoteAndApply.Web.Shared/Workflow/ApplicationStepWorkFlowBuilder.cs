using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Web.Shared.Workflow.Steps;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public static class ApplicationStepWorkFlowBuilder
    {
        private const int InterviewWeight = 35;

        public static IApplicationStepResolver BuildQuoteAndApplyWorkflow()
        {
            var basicInfoStep = new BasicInfoLetMePlayStep();
            var needsAnalysisStep = new NeedsAnalysisStep();
            var coverSelectionStepRiskOne = new SelectCoverStep(0);
            var qualificationStepRiskOne = new QualificationStep(0);
            var referStep = new ReferStep();
            var reviewStep = new ReviewStep();
            var summaryStep = new SummaryStep();
            var purchaseStep = new PurchaseStep();
            var confirmationStep = new ConfirmationStep();

            basicInfoStep
                .WithNextStep(coverSelectionStepRiskOne);

            needsAnalysisStep.WithNextStep(needsAnalysisStep);

            coverSelectionStepRiskOne
                .WithPreviousStep(basicInfoStep)
                    .When(ctx => ctx.Application.Source == PolicySource.CustomerPortalBuildMyOwn)
                .WithPreviousStep(needsAnalysisStep)
                    .When(ctx => ctx.Application.Source == PolicySource.CustomerPortalHelpMeChoose);

            coverSelectionStepRiskOne.WithNextStep(qualificationStepRiskOne);

            qualificationStepRiskOne.WithPreviousStep(coverSelectionStepRiskOne)
                .When(ctx => ctx.Application.Risks[0].InterviewStatus == InterviewStatus.NotStarted);

            qualificationStepRiskOne.WithNextStep(reviewStep);

            reviewStep.WithPreviousStep(qualificationStepRiskOne)
                .When(ctx => ctx.Application.Risks[0].InterviewStatus == InterviewStatus.Incomplete);
            reviewStep
                .WithNextStep(summaryStep);

            summaryStep.WithPreviousStep(reviewStep);
            summaryStep.WithNextStep(purchaseStep)
                .When(ctx => ctx.Application.Risks[0].InterviewStatus == InterviewStatus.Complete);
            summaryStep.WithNextStep(referStep)
                    .When(ctx => ctx.Application.Risks[0].InterviewStatus == InterviewStatus.Referred);

            purchaseStep.WithPreviousStep(reviewStep);
            purchaseStep.WithNextStep(confirmationStep);
           
            confirmationStep.WithPreviousStep(purchaseStep);

            var application = ApplicationStepConfiguration.CreateApplicationForProduct("QAPLMC",
                "Cover Builder: Let me play")
                .InitializeWorkflowTree(basicInfoStep)
                .WithStepIndicatorSteps(basicInfoStep, coverSelectionStepRiskOne, qualificationStepRiskOne,
                    reviewStep, confirmationStep);

            return application;
        }
    }
}
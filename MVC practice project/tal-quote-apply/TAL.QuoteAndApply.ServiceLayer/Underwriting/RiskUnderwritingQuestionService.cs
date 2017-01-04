using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Converters;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface IRiskUnderwritingQuestionService
    {
        UnderwritingPosition GetCurrentUnderwriting(int riskId);
        UnderwritingPosition GetCurrentUnderwritingRaw(int riskId);
        IEnumerable<UnderwritingQuestionsByBenefitCode> GetChoicePointQuestions(int riskId, IList<string> selectedBenefitCodes);
        IEnumerable<UnderwritingQuestionsByBenefitCode> GetQuestionsWithLoadings(int riskId, IList<string> selectedBenefitCodes);
        UnderwritingAnswerQuestionResult AnswerQuestionWithoutSyncing(int riskId, string questionId,
            IEnumerable<UnderwritingAnswer> selectedAnswers);
        UnderwritingAnswerQuestionResult AnswerQuestionWithoutSyncingRaw(int riskId, string questionId,
            IEnumerable<UnderwritingAnswer> selectedAnswers);
        UnderwritingAnswerQuestionResult AnswerQuestionAndSync(int riskId, string questionId,
            IEnumerable<UnderwritingAnswer> selectedAnswers);
    }

    public class RiskUnderwritingQuestionService : IRiskUnderwritingQuestionService
    {
        private readonly IGetUnderwritingInterview _getUnderwritingInterview;
        private readonly IUnderwritingModelConverter _underwritingModelConverter;
        private readonly IUnderwritingBenefitsResponseChangeSubject _benefitsResponseChangeSubject;
        private readonly IUpdateUnderwritingInterview _updateUnderwritingInterview;
        private readonly IRiskService _riskService;
        private readonly IRiskUnderwritingAnswerSyncService _answerSyncService;

        public RiskUnderwritingQuestionService(IGetUnderwritingInterview getUnderwritingInterview,
            IUnderwritingBenefitsResponseChangeSubject benefitsResponseChangeSubject,
            IUnderwritingModelConverter underwritingModelConverter,
            IUpdateUnderwritingInterview updateUnderwritingInterview,
            IRiskService riskService, IRiskUnderwritingAnswerSyncService answerSyncService)
        {
            _getUnderwritingInterview = getUnderwritingInterview;
            _benefitsResponseChangeSubject = benefitsResponseChangeSubject;
            _underwritingModelConverter = underwritingModelConverter;
            _updateUnderwritingInterview = updateUnderwritingInterview;
            _riskService = riskService;
            _answerSyncService = answerSyncService;
        }

        public UnderwritingPosition GetCurrentUnderwriting(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            var interview = _getUnderwritingInterview.GetInterview(risk.InterviewId, risk.InterviewConcurrencyToken, _benefitsResponseChangeSubject);

            return _underwritingModelConverter.From(interview);
        }

        public UnderwritingPosition GetCurrentUnderwritingRaw(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            var interview = _getUnderwritingInterview.GetInterview(risk.InterviewId, risk.InterviewConcurrencyToken, _benefitsResponseChangeSubject);

            return _underwritingModelConverter.WithNoFilteringFrom(interview);
        }

        public IEnumerable<UnderwritingQuestionsByBenefitCode> GetChoicePointQuestions(int riskId, IList<string> selectedBenefitCodes)
        {
            var risk = _riskService.GetRisk(riskId);
            var interview = _getUnderwritingInterview.GetInterviewWithoutSyncing(risk.InterviewId, risk.InterviewConcurrencyToken);

            foreach (var benefit in interview.Benefits)
            {
                if (!selectedBenefitCodes.Contains(benefit.BenefitCode))
                {
                    continue;
                }

                var choicePointQuestions =
                    benefit.AnsweredQuestions.Where(
                        q =>
                            q.Tags.Contains(QuestionTagConstants.ChoicePoint));

                yield return new UnderwritingQuestionsByBenefitCode(benefit.BenefitCode, choicePointQuestions.Select(_underwritingModelConverter.From));
            }
        }

        public IEnumerable<UnderwritingQuestionsByBenefitCode> GetQuestionsWithLoadings(int riskId, IList<string> selectedBenefitCodes)
        {
            var risk = _riskService.GetRisk(riskId);
            var interview = _getUnderwritingInterview.GetInterviewWithoutSyncing(risk.InterviewId, risk.InterviewConcurrencyToken);

            foreach (var benefit in interview.Benefits)
            {
                if (!selectedBenefitCodes.Contains(benefit.BenefitCode))
                {
                    continue;
                }

                //Get questions with loadings but not Choice Point questions
                var questionsWithLoadings =
                    benefit.AnsweredQuestions.Where(
                        q =>
                            !q.Tags.Contains(QuestionTagConstants.ChoicePoint) &&
                            q.Answers.Any(a => a.Selected && a.Loadings != null && a.Loadings.Any()));

                yield return
                    new UnderwritingQuestionsByBenefitCode(benefit.BenefitCode,
                        questionsWithLoadings.Select(_underwritingModelConverter.From));
            }
        }

        public UnderwritingAnswerQuestionResult AnswerQuestionWithoutSyncing(int riskId, string questionId,
            IEnumerable<UnderwritingAnswer> selectedAnswers)
        {
            var answerResult = AnswerQuestion(riskId, questionId, selectedAnswers);
            return _underwritingModelConverter.From(answerResult);
        }

        public UnderwritingAnswerQuestionResult AnswerQuestionWithoutSyncingRaw(int riskId, string questionId,
            IEnumerable<UnderwritingAnswer> selectedAnswers)
        {
            var answerResult = AnswerQuestion(riskId, questionId, selectedAnswers);
            return _underwritingModelConverter.WithNoFilteringFrom(answerResult);
        }

        public UnderwritingAnswerQuestionResult AnswerQuestionAndSync(int riskId, string questionId,
            IEnumerable<UnderwritingAnswer> selectedAnswers)
        {
            var answerResult = AnswerQuestion(riskId, questionId, selectedAnswers);

            var underwritingQuestionAnswer =
                new UnderwritingQuestionAnswer(questionId,
                    selectedAnswers.Select(x => new UnderwritingAnswer(x.Id, x.Text)));
            _answerSyncService.SyncRiskWithUnderwritingAnswer(riskId, answerResult.ConcurrencyToken,
                underwritingQuestionAnswer);

            return _underwritingModelConverter.From(answerResult);
        }

        private ReadOnlyUpdatedUnderwritingInterview AnswerQuestion(int riskId, string questionId,
            IEnumerable<UnderwritingAnswer> selectedAnswers)
        {
            var risk = _riskService.GetRisk(riskId);
            var updateUnderwritingResult = _updateUnderwritingInterview.AnswerQuestion(risk.InterviewId,
                risk.InterviewConcurrencyToken, questionId,
                selectedAnswers.Select(a => new AnswerSubmission { ResponseId = a.Id, Text = a.Text }).ToArray());

            _riskService.SetLatestConcurrencyToken(risk, updateUnderwritingResult.ConcurrencyToken);

            return updateUnderwritingResult;

        }

    }
}

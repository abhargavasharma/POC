using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Exceptions;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models;
using TAL.QuoteAndApply.ServiceLayer.Search.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.Underwriting;
using TAL.QuoteAndApply.Underwriting.Extensions;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface IRiskUnderwritingAnswerSyncService
    {
        void SyncRiskWithUnderwritingAnswer(int riskId, string concurrencyToken, UnderwritingQuestionAnswer questionAnswer);
        void SyncRiskWithFullInterviewAndUpdatePlanEligibility(int riskId);
    }

    public class RiskUnderwritingAnswerSyncService : IRiskUnderwritingAnswerSyncService
    {
        private readonly IKnownQuestionIdProvider _knownQuestionIdProvider;
        private readonly IRiskService _riskService;
        private readonly IGetUnderwritingInterview _getUnderwritingInterview;
        private readonly IDateOfBirthChangeSubject _dateOfBirthChangeSubject;
        private readonly IGenderChangeSubject _genderChangeSubject;
        private readonly IOccupationChangeSubject _occupationChangeSubject;
        private readonly IResidencyChangeSubject _residencyChangeSubject;
        private readonly ISmokerStatusChangeSubject _smokerStatusChangeSubject;
        private readonly IAnnualIncomeChangeSubject _annualIncomeChangeSubject;
        private readonly IUnderwritingBenefitsResponseChangeSubject _underwritingBenefitsResponseChangeSubject;
        private readonly IUnderwritingTagMetaDataService _underwritingTagMetaDataService;
        private readonly IPlanAutoUpdateService _planAutoUpdateService;

        public RiskUnderwritingAnswerSyncService(IKnownQuestionIdProvider knownQuestionIdProvider,
            IRiskService riskService,
            IGetUnderwritingInterview getUnderwritingInterview,
            IDateOfBirthChangeSubject dateOfBirthChangeSubject,
            IGenderChangeSubject genderChangeSubject,
            IOccupationChangeSubject occupationChangeSubject,
            IResidencyChangeSubject residencyChangeSubject,
            ISmokerStatusChangeSubject smokerStatusChangeSubject,
            IAnnualIncomeChangeSubject annualIncomeChangeSubject,
            IUnderwritingBenefitsResponseChangeSubject underwritingBenefitsResponseChangeSubject,
            IUnderwritingTagMetaDataService underwritingTagMetaDataService, IPlanAutoUpdateService planAutoUpdateService)
        {
            _knownQuestionIdProvider = knownQuestionIdProvider;
            _riskService = riskService;
            _getUnderwritingInterview = getUnderwritingInterview;
            _dateOfBirthChangeSubject = dateOfBirthChangeSubject;
            _genderChangeSubject = genderChangeSubject;
            _occupationChangeSubject = occupationChangeSubject;
            _residencyChangeSubject = residencyChangeSubject;
            _smokerStatusChangeSubject = smokerStatusChangeSubject;
            _annualIncomeChangeSubject = annualIncomeChangeSubject;
            _underwritingBenefitsResponseChangeSubject = underwritingBenefitsResponseChangeSubject;
            _underwritingTagMetaDataService = underwritingTagMetaDataService;
            _planAutoUpdateService = planAutoUpdateService;
        }

        public void SyncRiskWithUnderwritingAnswer(int riskId, string concurrencyToken, UnderwritingQuestionAnswer questionAnswer)
        {
            var risk = _riskService.GetRisk(riskId);

            if (risk == null)
            {
                throw new ServiceLayerException("Risk not found");
            }

            //get the interview regardless of if it is a known question. This will ensure we sync the cover uw status with the interview
            var interview = _getUnderwritingInterview.GetInterview(risk.InterviewId, concurrencyToken, _underwritingBenefitsResponseChangeSubject);
            _riskService.SetLatestConcurrencyToken(risk, concurrencyToken);

            var knownQuestions = _knownQuestionIdProvider.GetAllKnownQuestionIds(risk);
            if (knownQuestions.Contains(questionAnswer.QuestionId))
            {
                var answer = GetAnswerFromInterview(interview, questionAnswer.QuestionId);
                if (answer != null)
                {
                    PerformRiskUpdateActionForQuestion(questionAnswer.QuestionId, risk, answer);

                }
            }

            PerformRiskOccupationUpdate(risk, interview, questionAnswer);
            _planAutoUpdateService.UpdatePlansToConformWithPlanEligiblityRules(risk);
        }

        public void SyncRiskWithFullInterviewAndUpdatePlanEligibility(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);

            if (risk == null)
            {
                throw new ServiceLayerException("Risk not found");
            }

            //get the interview without passing a concurrency token
            var interview = _getUnderwritingInterview.GetInterview(risk.InterviewId, _underwritingBenefitsResponseChangeSubject);
            _riskService.SetLatestConcurrencyToken(risk, interview.ConcurrencyToken);

            var knownQuestions = _knownQuestionIdProvider.GetAllKnownQuestionIds(risk);

            //because we don't know why the interview has a new token we should resync our known questions and variables that are saved against the risk
            foreach (var quesitonId in knownQuestions)
            {
                var answer = GetAnswerFromInterview(interview, quesitonId);
                if (answer != null)
                {
                    PerformRiskUpdateActionForQuestion(quesitonId, risk, answer);
                }
            }

            PerformRiskOccupationUpdate(risk, interview);
            _planAutoUpdateService.UpdatePlansToConformWithPlanEligiblityRules(risk);
        }

        private void PerformRiskOccupationUpdate(IRisk risk, ReadOnlyUnderwritingInterview interview)
        {
            var industryAnswer = GetAnswerFromInterviewByTag(interview, QuestionTagConstants.IndustryQuestionTag);
            var occupationAnswer = GetAnswerFromInterviewByTag(interview, QuestionTagConstants.OccupationQuestionTag);
            
            if (industryAnswer != null && occupationAnswer != null)
            {
                _occupationChangeSubject.Notify(
                    new ChangeEnvelope(
                        BuildUpdateOccupationParam(risk, occupationAnswer, industryAnswer)
                    )
                );
            }
            else
            {
                _occupationChangeSubject.Notify(
                    new ChangeEnvelope(
                        UpdateOccupationParam.NoOccupation(risk.Id, risk.PartyId)
                        )
                );
            }

        }

        private void PerformRiskOccupationUpdate(IRisk risk, ReadOnlyUnderwritingInterview interview, UnderwritingQuestionAnswer questionAnswer)
        {
            var industryAnswer = GetAnswerFromInterviewByTag(interview, QuestionTagConstants.IndustryQuestionTag);
            var industryQuestion = interview.AllQuestions.SingleOrDefaultByTag(QuestionTagConstants.IndustryQuestionTag);
            var occupationAnswer = GetAnswerFromInterviewByTag(interview, QuestionTagConstants.OccupationQuestionTag);
            var occpationQuestion = interview.AllQuestions.SingleOrDefaultByTag(QuestionTagConstants.OccupationQuestionTag);

            if (industryAnswer != null && occupationAnswer != null &&
                (industryQuestion.Id == questionAnswer.QuestionId || occpationQuestion.Id == questionAnswer.QuestionId))
            {
                _occupationChangeSubject.Notify(
                    new ChangeEnvelope(
                        BuildUpdateOccupationParam(risk, occupationAnswer, industryAnswer)
                    )
                );
            }

            if (industryQuestion != null && questionAnswer.QuestionId == industryQuestion.Id)
            {
                _occupationChangeSubject.Notify(
                    new ChangeEnvelope(
                        UpdateOccupationParam.NoOccupation(risk.Id, risk.PartyId)
                        )
                );
            }
        }

        private UpdateOccupationParam BuildUpdateOccupationParam(IRisk risk, ReadOnlyAnswer occupationAnswer, ReadOnlyAnswer industryAnswer)
        {
            return new UpdateOccupationParam(
                risk.Id,
                risk.PartyId,
                _underwritingTagMetaDataService.GetFirstOrDefault(occupationAnswer.Tags,
                    AnswerTagConstants.TalConsumerOccupationClass, ""),
                occupationAnswer.ResponseId,
                occupationAnswer.SelectedText,
                industryAnswer.ResponseId,
                industryAnswer.SelectedText,
                _underwritingTagMetaDataService.TagAndValueExists(occupationAnswer.Tags,
                    AnswerTagConstants.TalConsumerOccupationTpdOwnAnyKey,
                    AnswerTagConstants.TalConsumerOccupationTpdAnyValue),
                _underwritingTagMetaDataService.TagAndValueExists(occupationAnswer.Tags,
                    AnswerTagConstants.TalConsumerOccupationTpdOwnAnyKey,
                    AnswerTagConstants.TalConsumerOccupationTpdOwnValue),
                _underwritingTagMetaDataService.GetFirstOrDefault(occupationAnswer.Tags,
                    AnswerTagConstants.TalConsumerOccupationTpdLoading, (decimal?)null),
                _underwritingTagMetaDataService.GetFirstOrDefault(occupationAnswer.Tags,
                    AnswerTagConstants.TalConsumerPasCode, ""));

        }

        private void PerformRiskUpdateActionForQuestion(string questionid, IRisk risk, ReadOnlyAnswer answer)
        {
            if (questionid == QuestionIds.DateOfBirthQuestion)
            {
                var dob = MapDateOfBirthAnswer(answer);
                if (dob.HasValue)
                {
                    _dateOfBirthChangeSubject.Notify(new ChangeEnvelope(new UpdateDateOfBirthParam(risk.Id, risk.PartyId, dob.Value)));
                }
            }
            else if (questionid == QuestionIds.GenderQuestion)
            {
                _genderChangeSubject.Notify(new ChangeEnvelope(new UpdateGenderParam(risk.Id, risk.PartyId, MapGenderAnswer(answer))));
            }
            else if (questionid == QuestionIds.ResidencyQuestion)
            {
                _residencyChangeSubject.Notify(new ChangeEnvelope(new UpdateResidencyParam(risk.Id, risk.PartyId, MapResidencyAnswer(answer))));
            }
            else if (questionid == QuestionIds.SmokerQuestion)
            {
                _smokerStatusChangeSubject.Notify(new ChangeEnvelope(new UpdateSmokerStatusParam(risk.Id, risk.PartyId, MapSmokerStatusAnswer(answer))));
            }
            else if (questionid == QuestionIds.AnnualIncomeQuestion)
            {
                _annualIncomeChangeSubject.Notify(new ChangeEnvelope(new UpdateAnnualIncomeParam(risk.Id, risk.PartyId, MapAnnualIncomeAnswer(answer))));
            }
        }

        private DateTime? MapDateOfBirthAnswer(ReadOnlyAnswer answer)
        {
            return answer.SelectedText.ToDateExcactDdMmYyyy();
        }

        private long MapAnnualIncomeAnswer(ReadOnlyAnswer answer)
        {
            return answer.SelectedText.ToLong(); //Note: This is where we are truncating Annual Income if decimals have been added in Phoenix
        }

        //todo: we have four options on the RiskDto object but only true/false in UW.
        private SmokerStatus MapSmokerStatusAnswer(ReadOnlyAnswer answer)
        {
            var smokerStatusHelper = new SmokerStatusHelper(answer.SelectedText);
            return smokerStatusHelper.Status;
        }

        private char MapGenderAnswer(ReadOnlyAnswer answer)
        {
            if (answer.SelectedText == "Female")
                return 'F';

            return 'M';
        }

        private ResidencyStatus MapResidencyAnswer(ReadOnlyAnswer answer)
        {
            if (answer.SelectedText == "No")
                return ResidencyStatus.NonAustralian;

            return ResidencyStatus.Australian;
        }

        private IDictionary<string, string> GetVariablesFromInterview(ReadOnlyUnderwritingInterview interview)
        {
            var variables = interview.Benefits.SelectMany(b => b.Variables).Distinct().ToDictionary(x => x.Key, y => y.Value);
            return variables;
        }

        private ReadOnlyAnswer GetAnswerFromInterview(ReadOnlyUnderwritingInterview interview, string questionid)
        {
            var questions = interview.Benefits.SelectMany(b => b.UnansweredQuestions.Concat(interview.Benefits.SelectMany(b1 => b1.AnsweredQuestions))).Distinct(DistinctQuestionComparer.Instance);

            var question = questions.FirstOrDefault(x => x.Id == questionid);
            if (question == null)
            {
                return null;
            }

            return question.Answers.FirstOrDefault(x => x.Selected);
        }

        private ReadOnlyAnswer GetAnswerFromInterviewByTag(ReadOnlyUnderwritingInterview interview, string questionTag)
        {
            var questions = interview.Benefits.SelectMany(b => b.UnansweredQuestions.Concat(interview.Benefits.SelectMany(b1 => b1.AnsweredQuestions))).Distinct(DistinctQuestionComparer.Instance);

            var question = questions.FirstOrDefault(x => x.Tags.Any(tag => tag.Equals(questionTag, StringComparison.OrdinalIgnoreCase)));
            if (question == null)
            {
                return null;
            }

            return question.Answers.FirstOrDefault(x => x.Selected);
        }
    }



    public class OccupationMetaData
    {
        public string IsTpdAny { get; set; }
        public string IsTpdOwn { get; set; }
        public decimal? TpdOccupationLoading { get; set; }
    }
}

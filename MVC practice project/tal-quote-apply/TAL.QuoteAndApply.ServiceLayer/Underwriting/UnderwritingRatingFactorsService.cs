using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Search.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.Underwriting.Extensions;
using TAL.QuoteAndApply.Underwriting.Models.Converters;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Talus;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface IUnderwritingRatingFactorsService
    {
        InterviewReferenceInformation UpdateUnderwritingWithRatingFactorValues(string interviewId, string templateVersion, string concurrencyToken, RatingFactorsParam ratingFactorsParam);
        InterviewReferenceInformation UpdateUnderwritingWithRatingFactorValues(IRisk risk, RatingFactorsParam ratingFactorsParam);
    }

    public class UnderwritingRatingFactorsService : IUnderwritingRatingFactorsService
    {
        private readonly IUpdateUnderwritingInterview _updateUnderwritingInterview;
        private readonly IUnderwritingBenefitsResponseChangeSubject _underwritingBenefitsResponseChangeSubject;
        private readonly IUnderwritingBenefitResponsesChangeParamConverter _underwritingBenefitResponsesChangeParamConverter;
        private readonly IGetUnderwritingInterview _getUnderwritingInterview;
        private readonly IUnderwritingQuestionAnswerProvider _getUnderwritingQuestionAnswerProvider;


        public UnderwritingRatingFactorsService(IUpdateUnderwritingInterview updateUnderwritingInterview,
            IUnderwritingBenefitsResponseChangeSubject underwritingBenefitsResponseChangeSubject,
            IUnderwritingBenefitResponsesChangeParamConverter underwritingBenefitResponsesChangeParamConverter, IGetUnderwritingInterview getUnderwritingInterview, IUnderwritingQuestionAnswerProvider getUnderwritingQuestionAnswerProvider)
        {
            _updateUnderwritingInterview = updateUnderwritingInterview;
            _underwritingBenefitsResponseChangeSubject = underwritingBenefitsResponseChangeSubject;
            _underwritingBenefitResponsesChangeParamConverter = underwritingBenefitResponsesChangeParamConverter;
            _getUnderwritingInterview = getUnderwritingInterview;
            _getUnderwritingQuestionAnswerProvider = getUnderwritingQuestionAnswerProvider;
        }

        public InterviewReferenceInformation UpdateUnderwritingWithRatingFactorValues(string interviewId, string templateVersion, string concurrencyToken,
            RatingFactorsParam ratingFactorsParam)
        {
            Func<ReadOnlyUnderwritingInterview> funcGetInterview = () => _getUnderwritingInterview.GetInterview(interviewId, concurrencyToken,
                _underwritingBenefitsResponseChangeSubject);

            var dobQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.DateOfBirthQuestionTag,
                    templateVersion, funcGetInterview).Single();
            var genderQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.GenderQuestionTag,
                                templateVersion, funcGetInterview);


            var initialQuestions = new List<UnderwritingInitialiseQuestion>
            {
                new UnderwritingInitialiseFreetextQuestion(ratingFactorsParam.DateOfBirth.ToString("dd/MM/yyyy"), interviewId,
                    dobQuestion, _updateUnderwritingInterview),
            };

            genderQuestion.Do(
                q => initialQuestions.Add(new UnderwritingInitialiseGenderQuestion(ratingFactorsParam.Gender, interviewId,
                        q, _updateUnderwritingInterview)));

            // Adding question only if the AustralianResident is not null
            if (ratingFactorsParam.IsAustralianResident != null)
            {
                var residentQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.ResidencyQuestionTag,
                                templateVersion, funcGetInterview).Single();

                initialQuestions.Add(new UnderwritingInitialiseYesNoQuestion(ratingFactorsParam.IsAustralianResident.Value, interviewId,
                    residentQuestion, _updateUnderwritingInterview));
            }

            if (ratingFactorsParam.SmokerStatus != null)
            {
                var smokerQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.SmokerQuestionTag,
                    templateVersion, funcGetInterview).Single();

                initialQuestions.Add(new UnderwritingInitialiseYesNoQuestion(ratingFactorsParam.SmokerStatus.IsSmoker,
                    interviewId, smokerQuestion, _updateUnderwritingInterview));
            }

            var incomeQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.AnnualIncomeQuestionTag,
                    templateVersion, funcGetInterview).SingleOrDefault();

            if (incomeQuestion != null)
            {
                initialQuestions.Add(new UnderwritingInitialiseFreetextQuestion(ratingFactorsParam.Income.ToString(), interviewId,
                    incomeQuestion, _updateUnderwritingInterview));
            }

            //answer all the questions
            var latestAnswerResult = AnswerQuestions(initialQuestions, concurrencyToken);
            concurrencyToken = latestAnswerResult.ConcurrencyToken;
            ReadOnlyQuestion industryQuestion = null;
            ReadOnlyQuestion occupationQuestion = null;

            if (!string.IsNullOrEmpty(ratingFactorsParam.IndustryCode))
            {
                industryQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.IndustryQuestionTag,
                        templateVersion, funcGetInterview).Single();
                latestAnswerResult = _updateUnderwritingInterview.AnswerQuestion(interviewId, latestAnswerResult.ConcurrencyToken,
                    industryQuestion.Id, new AnswerSubmission { ResponseId = ratingFactorsParam.IndustryCode });
                concurrencyToken = latestAnswerResult.ConcurrencyToken;

                if (latestAnswerResult != null)
                {
                    occupationQuestion = latestAnswerResult.AllAddedQuestions.SingleOrDefaultByTag(QuestionTagConstants.OccupationQuestionTag) ??
                                             _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.OccupationQuestionTag,
                        templateVersion, funcGetInterview).Single();

                    latestAnswerResult = _updateUnderwritingInterview.AnswerQuestion(interviewId,
                        latestAnswerResult.ConcurrencyToken,
                        occupationQuestion.Id, new AnswerSubmission { ResponseId = ratingFactorsParam.OccupationCode });
                    concurrencyToken = latestAnswerResult.ConcurrencyToken;

                }
            }

            var benefitResponseChangeParam = _underwritingBenefitResponsesChangeParamConverter.CreateFrom(interviewId, latestAnswerResult);

            return new InterviewReferenceInformation(interviewId, templateVersion, concurrencyToken,
                new InterviewOccupationInformation(industryQuestion, ratingFactorsParam.IndustryCode, occupationQuestion,
                    ratingFactorsParam.OccupationCode),
                benefitResponseChangeParam.BenefitResponseStatuses);
            //re-get the interview to sync everything back up in our system
        }

        public InterviewReferenceInformation UpdateUnderwritingWithRatingFactorValues(IRisk risk, RatingFactorsParam ratingFactorsParam)
        {
            var interviewId = risk.InterviewId;
            var concurrencyToken = risk.InterviewConcurrencyToken;
            var templateVersion = risk.InterviewTemplateVersion;

            Func<ReadOnlyUnderwritingInterview> funcGetInterview = () => _getUnderwritingInterview.GetInterview(interviewId, concurrencyToken,
                            _underwritingBenefitsResponseChangeSubject);

            var initialQuestions = new List<UnderwritingInitialiseQuestion>();

            if (ratingFactorsParam.DateOfBirth != risk.DateOfBirth)
            {
                var dobQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.DateOfBirthQuestionTag,
                    templateVersion, funcGetInterview).Single();
                initialQuestions.Add(new UnderwritingInitialiseFreetextQuestion(ratingFactorsParam.DateOfBirth.ToString("dd/MM/yyyy"), interviewId,
                    dobQuestion, _updateUnderwritingInterview));
            }

            if (ratingFactorsParam.Gender.MapToGender() != risk.Gender)
            {
                var genderQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.GenderQuestionTag,
                                templateVersion, funcGetInterview);


                genderQuestion.Do(
                    q => initialQuestions.Add(new UnderwritingInitialiseGenderQuestion(ratingFactorsParam.Gender, interviewId,
                            q, _updateUnderwritingInterview)));
            }

            if (ratingFactorsParam.IsAustralianResident != null)
            {
                var residentQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.ResidencyQuestionTag,
                                templateVersion, funcGetInterview).Single();

                initialQuestions.Add(new UnderwritingInitialiseYesNoQuestion(ratingFactorsParam.IsAustralianResident.Value, interviewId,
                    residentQuestion, _updateUnderwritingInterview));
            }

            if (ratingFactorsParam.SmokerStatus != null && ratingFactorsParam.SmokerStatus.Status != risk.SmokerStatus)
            {
                var smokerQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.SmokerQuestionTag,
                    templateVersion, funcGetInterview).Single();

                initialQuestions.Add(new UnderwritingInitialiseYesNoQuestion(ratingFactorsParam.SmokerStatus.IsSmoker,
                    interviewId, smokerQuestion, _updateUnderwritingInterview));
            }

            var incomeQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.AnnualIncomeQuestionTag,
                    templateVersion, funcGetInterview).SingleOrDefault();

            if (incomeQuestion != null && ratingFactorsParam.Income != risk.AnnualIncome)
            {
                initialQuestions.Add(new UnderwritingInitialiseFreetextQuestion(ratingFactorsParam.Income.ToString(), interviewId,
                    incomeQuestion, _updateUnderwritingInterview));
            }

            //answer all the questions
            var latestAnswerResult = AnswerQuestions(initialQuestions, risk.InterviewConcurrencyToken);
            concurrencyToken = latestAnswerResult.ConcurrencyToken;
            ReadOnlyQuestion industryQuestion = null;
            ReadOnlyQuestion occupationQuestion = null;

            if ((!string.IsNullOrEmpty(ratingFactorsParam.IndustryCode) && ratingFactorsParam.IndustryCode != risk.IndustryCode) ||
                (!string.IsNullOrEmpty(ratingFactorsParam.OccupationCode) && ratingFactorsParam.OccupationCode != risk.OccupationCode))
            {
                industryQuestion = _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.IndustryQuestionTag,
                        templateVersion, funcGetInterview).Single();
                latestAnswerResult = _updateUnderwritingInterview.AnswerQuestion(interviewId, latestAnswerResult.ConcurrencyToken,
                    industryQuestion.Id, new AnswerSubmission { ResponseId = ratingFactorsParam.IndustryCode });
                concurrencyToken = latestAnswerResult.ConcurrencyToken;

                if (latestAnswerResult != null)
                {
                    occupationQuestion = latestAnswerResult.AllAddedQuestions.SingleOrDefaultByTag(QuestionTagConstants.OccupationQuestionTag) ??
                                             _getUnderwritingQuestionAnswerProvider.GetQuestionByTag(QuestionTagConstants.OccupationQuestionTag,
                        templateVersion, funcGetInterview).Single();

                    latestAnswerResult = _updateUnderwritingInterview.AnswerQuestion(interviewId,
                        latestAnswerResult.ConcurrencyToken,
                        occupationQuestion.Id, new AnswerSubmission { ResponseId = ratingFactorsParam.OccupationCode });
                    concurrencyToken = latestAnswerResult.ConcurrencyToken;
                }
            }

            var benefitResponseChangeParam = _underwritingBenefitResponsesChangeParamConverter.CreateFrom(interviewId, latestAnswerResult);

            return new InterviewReferenceInformation(interviewId, templateVersion, concurrencyToken,
                new InterviewOccupationInformation(industryQuestion, ratingFactorsParam.IndustryCode, occupationQuestion,
                    ratingFactorsParam.OccupationCode),
                benefitResponseChangeParam.BenefitResponseStatuses);

        }

        private ReadOnlyUpdatedUnderwritingInterview AnswerQuestions(IEnumerable<UnderwritingInitialiseQuestion> initialiseQuestions, string concurrencyToken, IRisk risk = null)
        {
            var tmpConcurrencyToken = concurrencyToken;
            ReadOnlyUpdatedUnderwritingInterview latestAnswerQuestionResult = null;
            foreach (var initialiseQuestion in initialiseQuestions)
            {
                latestAnswerQuestionResult = initialiseQuestion.AnswerQuestion(tmpConcurrencyToken);
                tmpConcurrencyToken = latestAnswerQuestionResult.ConcurrencyToken;
            }

            return latestAnswerQuestionResult;
        }
    }
}

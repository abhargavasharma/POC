using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.Underwriting.Extensions;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface IKnownQuestionIdProvider
    {
        IEnumerable<string> GetAllKnownQuestionIds(IRisk risk);
    }

    public class KnownQuestionIdProvider : IKnownQuestionIdProvider
    {
        private readonly ICachingWrapper _cachingWrapper;
        private readonly IGetUnderwritingInterview _getUnderwritingInterview;
        private readonly IUnderwritingBenefitsResponseChangeSubject _underwritingBenefitsResponseChangeSubject;

        public KnownQuestionIdProvider(ICachingWrapper cachingWrapper, IGetUnderwritingInterview getUnderwritingInterview, IUnderwritingBenefitsResponseChangeSubject underwritingBenefitsResponseChangeSubject)
        {
            _cachingWrapper = cachingWrapper;
            _getUnderwritingInterview = getUnderwritingInterview;
            _underwritingBenefitsResponseChangeSubject = underwritingBenefitsResponseChangeSubject;
        }

        public IEnumerable<string> GetAllKnownQuestionIdsWithoutCache(IRisk risk, ReadOnlyUnderwritingInterview interview)
        {
            var allQuestions = interview.Benefits.SelectMany(b => b.AnsweredQuestions)
                .Concat(interview.Benefits.SelectMany(b => b.UnansweredQuestions))
                .DistinctBy(q => q.Id).ToArray();
            
            return new[]
            {
                allQuestions.SingleOrDefaultByTag(QuestionTagConstants.DateOfBirthQuestionTag)?.Id,
                allQuestions.SingleOrDefaultByTag(QuestionTagConstants.AnnualIncomeQuestionTag)?.Id,
                allQuestions.SingleOrDefaultByTag(QuestionTagConstants.ResidencyQuestionTag)?.Id,
                allQuestions.SingleOrDefaultByTag(QuestionTagConstants.SmokerQuestionTag)?.Id
            }
            .Concat(allQuestions.ContainsTag(QuestionTagConstants.GenderQuestionTag).Select(q => q.Id))
            .Where(qid => qid != null);
        }

        public IEnumerable<string> GetAllKnownQuestionIds(IRisk risk)
        {
            var interview = _cachingWrapper.GetOrAddCacheItemSliding(this.GetType(), $"Interview-{risk.InterviewId}",
                TimeSpan.FromMinutes(5), () => _getUnderwritingInterview.GetInterview(risk.InterviewId, risk.InterviewConcurrencyToken,
                    _underwritingBenefitsResponseChangeSubject));

            return _cachingWrapper.GetOrAddCacheItemSliding(this.GetType(), $"InterviewTemplate-{interview.TemplateId}", TimeSpan.FromHours(24), 
                () => GetAllKnownQuestionIdsWithoutCache(risk, interview));
        }
    }
}

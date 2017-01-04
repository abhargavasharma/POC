using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Underwriting.Extensions;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.Underwriting.Service
{
    public interface IUnderwritingQuestionAnswerProvider
    {
        IEnumerable<ReadOnlyQuestion> GetQuestionByTag(string tag, string templateVersion, Func<ReadOnlyUnderwritingInterview> lazyGetInterview);
    }

    public class UnderwritingQuestionAnswerProvider : IUnderwritingQuestionAnswerProvider
    {
        private readonly ICachingWrapper _cachingWrapper;

        public UnderwritingQuestionAnswerProvider(ICachingWrapper cachingWrapper)
        {
            _cachingWrapper = cachingWrapper;
        }

        public IEnumerable<ReadOnlyQuestion> GetQuestionByTag(string tag, string templateVersion, Func<ReadOnlyUnderwritingInterview> lazyGetInterview)
        {
            return _cachingWrapper.GetOrAddCacheItemIndefinite(this.GetType(), $"{templateVersion}-{tag}", () =>
            {
                var interview = lazyGetInterview.Invoke();
                var question = interview.AllQuestions.ContainsTag(tag);
                return question;
            });
        }
    }
}
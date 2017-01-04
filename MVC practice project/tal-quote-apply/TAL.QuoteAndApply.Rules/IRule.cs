using System;
using System.Linq.Expressions;

namespace TAL.QuoteAndApply.Rules
{
    public interface IRule<in T>
    {
        RuleResult IsSatisfiedBy(T target);
    }
}

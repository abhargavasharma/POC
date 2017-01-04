using System.Collections.Generic;
using System.Linq;

namespace TAL.QuoteAndApply.Rules
{
    public static class RuleExtensions
    {
        /// <summary>
        /// Calls IsSatisfiedBy called for all rules on the target object and returns true if they all passed
        /// </summary>
        public static RuleResult AllAreSatisfied<TRule, TTarget>(this IEnumerable<TRule> rulesToSatisfy,
            TTarget objectToValidate)
            where TRule : IRule<TTarget>
        {
            var results = rulesToSatisfy.Select(rule => rule.IsSatisfiedBy(objectToValidate)).ToList();
            var isSatisfied = results.All(r => r.IsSatisfied);

            // Return true if all are ok, and put together all the failed messages
            return RuleResult.ToResult(isSatisfied, results.SelectMany(r => r.Messages).ToArray());
        }

        /// <summary>
        /// Calls IsSatisfiedBy called for all rules on the target object and returns true if they all passed
        /// </summary>
        public static RuleResult AllAreSatisfied<TRule, TTarget>(this IEnumerable<TRule> rulesToSatisfy,
            TTarget objectToValidate, string validationKey)
            where TRule : IRule<TTarget>
        {
            var results = rulesToSatisfy.Select(rule => rule.IsSatisfiedBy(objectToValidate)).ToList();
            var isSatisfied = results.All(r => r.IsSatisfied);

            // Return true if all are ok, and put together all the failed messages
            return RuleResult.ToResult(validationKey, isSatisfied, results.SelectMany(r => r.Messages).ToArray());
        }
    }
}
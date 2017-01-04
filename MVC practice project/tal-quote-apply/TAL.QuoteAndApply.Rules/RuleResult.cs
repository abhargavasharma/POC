using System;
using System.Linq;

namespace TAL.QuoteAndApply.Rules
{
    public struct RuleResult
    {
        public string Key;
        public bool IsSatisfied;
        public string[] Messages;

        public bool IsBroken
        {
            get { return !IsSatisfied; }
        }

        public string DefaultMessage()
        {
            return Messages.FirstOrDefault();
        }

        public static RuleResult ToResult(bool valid, params string[] messages)
        {
            return new RuleResult(valid, messages);
        }

        public static RuleResult ToResult(string validationKey, bool valid, params string[] messages)
        {
            return new RuleResult(validationKey, valid,  messages);
        }

        public static RuleResult NotSatisfied(string message)
        {
            return ToResult(false, message);
        }

        public RuleResult(bool isSatisfied, params string[] messages)
        {
            IsSatisfied = isSatisfied;
            Messages = !isSatisfied ? messages : new string[0];
            Key = String.Empty;
        }

        public RuleResult(string key, bool isSatisfied, params string[] messages)
        {

            IsSatisfied = isSatisfied;
            Messages = !isSatisfied ? messages : new string[0];
            Key = key;
        }

        public static implicit operator bool(RuleResult result)
        {
            return result.IsSatisfied;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Rules;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Party.Services
{
    public interface IPartyRulesService
    {
        IEnumerable<RuleResult> ValidatePartyForSave(IParty party);
        IEnumerable<RuleResult> ValidatePartyForInforce(IParty party);
        IEnumerable<RuleResult> ValidatePartyForCreateLead(IParty party);
    }

    public class PartyRulesService : IPartyRulesService
    {
        private readonly IPartyRuleFactory _ruleFactory;

        public PartyRulesService(IPartyRuleFactory ruleFactory)
        {
            _ruleFactory = ruleFactory;
        }

        public IEnumerable<RuleResult> ValidatePartyForSave(IParty party)
        {
            var firstNameRules = _ruleFactory.GetStringIsOnlyLettersSpacesHyphensRule("Party.FirstName");
            var surnameRules = _ruleFactory.GetStringIsOnlyLettersSpacesHyphensRule("Party.Surname");

            var postcodeIsNumbersRule = _ruleFactory.GetStringIsNumbersOnlyRule("Party.Postcode");
            var postcodeIsLengthRule = _ruleFactory.GetStringIsBetweenMinAndMaxLengthRule("Party.Postcode", 4, 4);

            var mobileNumberIsNumbersRule = _ruleFactory.GetStringIsNumbersOnlyRule("Party.MobileNumber");
            var mobileNumberIsLengthRule = _ruleFactory.GetStringIsBetweenMinAndMaxLengthRule("Party.MobileNumber", 10, 10);
            var mobileNumberAreaCodeRule = _ruleFactory.GetIsValidMobilePrefixRule("Party.MobileNumber");

            var homeNumberIsNumbersRule = _ruleFactory.GetStringIsNumbersOnlyRule("Party.HomeNumber");
            var homeNumberIsLengthRule = _ruleFactory.GetStringIsBetweenMinAndMaxLengthRule("Party.HomeNumber", 10, 10);
            var homeNumberAreaCodeRule = _ruleFactory.GetStartsWithPhoneAreaCodeRule("Party.HomeNumber");

            var emailAddressRule = _ruleFactory.GetValidEmailAddressRule("Party.EmailAddress");

            yield return firstNameRules.IsSatisfiedBy(party.FirstName);
            yield return surnameRules.IsSatisfiedBy(party.Surname);
            yield return postcodeIsNumbersRule.IsSatisfiedBy(party.Postcode);
            yield return postcodeIsLengthRule.IsSatisfiedBy(party.Postcode);
            yield return mobileNumberIsNumbersRule.IsSatisfiedBy(party.MobileNumber);
            yield return mobileNumberIsLengthRule.IsSatisfiedBy(party.MobileNumber);
            yield return mobileNumberAreaCodeRule.IsSatisfiedBy(party.MobileNumber);
            yield return homeNumberIsNumbersRule.IsSatisfiedBy(party.HomeNumber);
            yield return homeNumberIsLengthRule.IsSatisfiedBy(party.HomeNumber);
            yield return homeNumberAreaCodeRule.IsSatisfiedBy(party.HomeNumber);
            yield return emailAddressRule.IsSatisfiedBy(party.EmailAddress);
        }

        private RuleResult GetRuleResult<TObj, TRuleType>(Func<string, IRule<TRuleType>> getRule,
            TObj obj, Expression<Func<TObj, TRuleType>> propertyExpression)
        {
            var propertyName = propertyExpression.GetName().ToCamelCase();
            var propertyValue = propertyExpression.GetValue(obj);
            return getRule.Invoke(propertyName).IsSatisfiedBy(propertyValue);
        }

        public IEnumerable<RuleResult> ValidatePartyForInforce(IParty party)
        {
            var rulesResults = ValidatePartyForSave(party).ToList();

            rulesResults.AddRange(new[]
            {

                GetRuleResult(_ruleFactory.GetTitleIsNotUnkownRule, party, p => p.Title),
                GetRuleResult(_ruleFactory.GetStringHasValueRule, party, p => p.FirstName),
                GetRuleResult(_ruleFactory.GetStringHasValueRule, party, p => p.Surname),
                GetRuleResult(_ruleFactory.GetStringHasValueRule, party, p => p.Address),
                GetRuleResult(_ruleFactory.GetStringHasValueRule, party, p => p.Suburb),
                GetRuleResult(_ruleFactory.GetStringHasValueRule, party, p => p.Postcode),
                GetRuleResult(_ruleFactory.GetStringHasValueRule, party, p => p.MobileNumber),
                GetRuleResult(_ruleFactory.GetStringHasValueRule, party, p => p.EmailAddress),
                GetRuleResult(_ruleFactory.GetStateIsNotUnkownRule, party, p => p.State)

            });

            return rulesResults;
        }

        public IEnumerable<RuleResult> ValidatePartyForCreateLead(IParty party)
        {
            var ruleResults = new List<RuleResult>
            {
                GetRuleResult(_ruleFactory.GetStringHasValueRule, party, p => p.FirstName),
                GetRuleResult(_ruleFactory.GetStringHasValueRule, party, p => p.Surname),
                GetRuleResult(_ruleFactory.GetStringHasValueRule, party, p => p.EmailAddress)
            };

            return ruleResults;
        }
    }
}

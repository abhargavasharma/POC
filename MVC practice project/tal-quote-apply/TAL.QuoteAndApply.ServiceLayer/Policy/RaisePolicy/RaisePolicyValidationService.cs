using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Rules;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyValidationService
    {
        IEnumerable<RuleResult> ValidatePartyForInforce(IParty party);
        IEnumerable<RuleResult> ValidateOwnerForInforce(RaisePolicyOwner owner);
        IEnumerable<RuleResult> ValidateOwnerExternalCustomerRefForInforce(string externalCustomerReference);
    }
    public class RaisePolicyValidationService: IRaisePolicyValidationService
    {
        private readonly IPartyRuleFactory _ruleFactory;

        public RaisePolicyValidationService(IPartyRuleFactory ruleFactory)
        {
            _ruleFactory = ruleFactory;
        }

        public IEnumerable<RuleResult> ValidatePartyForInforce(IParty party)
        {
            var firstNameRules = _ruleFactory.GetStringIsOnlyLettersSpacesHyphensRule("Party.FirstName");
            var surnameRules = _ruleFactory.GetStringIsOnlyLettersSpacesHyphensRule("Party.Surname");

            var postcodeIsNumbersRule = _ruleFactory.GetStringIsNumbersOnlyRule("Party.Postcode");
            var postcodeIsLengthRule = _ruleFactory.GetStringIsBetweenMinAndMaxLengthRule("Party.Postcode", 4, 4);

            var titleIsNotUnknownRule = _ruleFactory.GetTitleIsNotUnkownRule("Party.Title");
            var firstNameEnteredRule = _ruleFactory.GetStringHasValueRule("Party.FirstName");
            var surnameIsEnteredRule = _ruleFactory.GetStringHasValueRule("Party.Surname");
            var postcodeIsEnteredRule = _ruleFactory.GetStringHasValueRule("Party.Postcode");
            


            yield return firstNameRules.IsSatisfiedBy(party.FirstName);
            yield return surnameRules.IsSatisfiedBy(party.Surname);
            yield return postcodeIsNumbersRule.IsSatisfiedBy(party.Postcode);
            yield return postcodeIsLengthRule.IsSatisfiedBy(party.Postcode);
            
            yield return titleIsNotUnknownRule.IsSatisfiedBy(party.Title);
            yield return firstNameEnteredRule.IsSatisfiedBy(party.FirstName);
            yield return surnameIsEnteredRule.IsSatisfiedBy(party.Surname);
            yield return postcodeIsEnteredRule.IsSatisfiedBy(party.Postcode);
        }

        public IEnumerable<RuleResult> ValidateOwnerForInforce(RaisePolicyOwner owner)
        {
            var firstNameRules = _ruleFactory.GetStringIsOnlyLettersSpacesHyphensRule("Owner.FirstName");
            var surnameRules = _ruleFactory.GetStringIsOnlyLettersSpacesHyphensRule("Owner.Surname");

            var postcodeIsNumbersRule = _ruleFactory.GetStringIsNumbersOnlyRule("Owner.Postcode");
            var postcodeIsLengthRule = _ruleFactory.GetStringIsBetweenMinAndMaxLengthRule("Owner.Postcode", 4, 4);

            var mobileNumberIsNumbersRule = _ruleFactory.GetStringIsNumbersOnlyRule("Owner.MobileNumber");
            var mobileNumberIsLengthRule = _ruleFactory.GetStringIsBetweenMinAndMaxLengthRule("Owner.MobileNumber", 10, 10);
            var mobileNumberAreaCodeRule = _ruleFactory.GetIsValidMobilePrefixRule("Owner.MobileNumber");

            var homeNumberIsNumbersRule = _ruleFactory.GetStringIsNumbersOnlyRule("Owner.HomeNumber");
            var homeNumberIsLengthRule = _ruleFactory.GetStringIsBetweenMinAndMaxLengthRule("Owner.HomeNumber", 10, 10);
            var homeNumberAreaCodeRule = _ruleFactory.GetStartsWithPhoneAreaCodeRule("Owner.HomeNumber");

            var emailAddressRule = _ruleFactory.GetValidEmailAddressRule("Owner.EmailAddress");

            var titleIsNotUnknownRule = _ruleFactory.GetTitleIsNotUnkownRule("Owner.Title");
            var firstNameEnteredRule = _ruleFactory.GetStringHasValueRule("Owner.FirstName");
            var surnameIsEnteredRule = _ruleFactory.GetStringHasValueRule("Owner.Surname");
            var addressIsEnteredRule = _ruleFactory.GetStringHasValueRule("Owner.Address");
            var suburbIsEnteredRule = _ruleFactory.GetStringHasValueRule("Owner.Suburb");
            var postcodeIsEnteredRule = _ruleFactory.GetStringHasValueRule("Owner.Postcode");
            var mobileIsEnteredRule = _ruleFactory.GetStringHasValueRule("Owner.MobileNumber");
            var emailIsEnteredRule = _ruleFactory.GetStringHasValueRule("Owner.EmailAddress");
            var stateIsNotUnknownRule = _ruleFactory.GetStateIsNotUnkownRule("Owner.State");

            var fundNameIsEnteredRule = _ruleFactory.GetStringHasValueRule("Owner.FundName");

            yield return firstNameRules.IsSatisfiedBy(owner.FirstName);
            yield return surnameRules.IsSatisfiedBy(owner.Surname);
            yield return postcodeIsNumbersRule.IsSatisfiedBy(owner.Postcode);
            yield return postcodeIsLengthRule.IsSatisfiedBy(owner.Postcode);
            yield return mobileNumberIsNumbersRule.IsSatisfiedBy(owner.MobileNumber);
            yield return mobileNumberIsLengthRule.IsSatisfiedBy(owner.MobileNumber);
            yield return mobileNumberAreaCodeRule.IsSatisfiedBy(owner.MobileNumber);
            yield return homeNumberIsNumbersRule.IsSatisfiedBy(owner.HomeNumber);
            yield return homeNumberIsLengthRule.IsSatisfiedBy(owner.HomeNumber);
            yield return homeNumberAreaCodeRule.IsSatisfiedBy(owner.HomeNumber);
            yield return emailAddressRule.IsSatisfiedBy(owner.EmailAddress);

            yield return titleIsNotUnknownRule.IsSatisfiedBy(owner.Title);
            yield return firstNameEnteredRule.IsSatisfiedBy(owner.FirstName);
            yield return surnameIsEnteredRule.IsSatisfiedBy(owner.Surname);
            yield return addressIsEnteredRule.IsSatisfiedBy(owner.Address);
            yield return suburbIsEnteredRule.IsSatisfiedBy(owner.Suburb);
            yield return postcodeIsEnteredRule.IsSatisfiedBy(owner.Postcode);
            yield return mobileIsEnteredRule.IsSatisfiedBy(owner.MobileNumber);
            yield return emailIsEnteredRule.IsSatisfiedBy(owner.EmailAddress);
            yield return stateIsNotUnknownRule.IsSatisfiedBy(owner.State);

            if (owner.OwnerType == PolicyOwnerType.SelfManagedSuperFund)
            {
                yield return fundNameIsEnteredRule.IsSatisfiedBy(owner.FundName);
            }
            
        }

        public IEnumerable<RuleResult> ValidateOwnerExternalCustomerRefForInforce(string externalCustomerReference)
        {
            var externalCustomerReferenceIsEnteredRule = _ruleFactory.GetStringHasValueRule("Owner.ExternalCustomerReference");
            yield return externalCustomerReferenceIsEnteredRule.IsSatisfiedBy(externalCustomerReference);
        }

    }
}

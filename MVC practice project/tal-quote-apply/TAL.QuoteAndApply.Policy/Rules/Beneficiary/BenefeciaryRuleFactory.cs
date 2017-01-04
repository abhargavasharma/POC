using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Beneficiary
{
    public interface IBenefeciaryRuleFactory : IGenericRuleFactory
    {
        IRule<IEnumerable<IBeneficiary>> GetSumOfBenefitShareRule(string key);
        IRule<State> GetStateRuleRequiredRule(string key);
        IRule<Title> GetTitleRequiredRule(string key);
        IRule<DateTime?> GetDateOfBirthRule(string key);
        IRule<float> GetBeneficiaryBenefitAmountRule(string key);
        IRule<DateTime?> GetDateOfBirthMaxAgeRule(string key);
        IRule<IEnumerable<IBeneficiary>> GetAtLeastOneBeneficiaryRequiredRule(string key);
    }

    public class BenefeciaryRuleFactory : GenericRuleFactory, IBenefeciaryRuleFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public BenefeciaryRuleFactory(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public IRule<IEnumerable<IBeneficiary>> GetAtLeastOneBeneficiaryRequiredRule(string key)
        {
            return new AtLeastOneBeneficiaryRequiredRule(key);
        }

        public IRule<IEnumerable<IBeneficiary>> GetSumOfBenefitShareRule(string key)
        {
            return new BeneficiarySumOfBenefitRule(key);
        }

        public IRule<float> GetBeneficiaryBenefitAmountRule(string key)
        {
            return new BeneficiaryBenefitAmountRule(key);
        }

        public IRule<Title> GetTitleRequiredRule(string key)
        {
            return new BeneficiaryTitleSetRule(key);
        }

        public IRule<State> GetStateRuleRequiredRule(string key)
        {
            return new BeneficiaryStateSetRule(key);
        }

        public IRule<DateTime?> GetDateOfBirthRule(string key)
        {
            return new BeneficiaryDateOfBirthSetRule(_dateTimeProvider, key);
        }

        public IRule<DateTime?> GetDateOfBirthMaxAgeRule(string key)
        {
            return new BeneficiaryDateOfBirthMaxAgeRule(_dateTimeProvider, key);
        } 
    }
}

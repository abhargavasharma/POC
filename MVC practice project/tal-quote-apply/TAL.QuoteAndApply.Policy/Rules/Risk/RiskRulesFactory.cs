using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Rules.Risk
{
    public interface IRiskRulesFactory
    {
        IRule<ResidencyStatus> GetResidencyStatusRequiredRule(string key);
        IRule<SmokerStatus> GetSmokerStatusRequiredRule(string key);
        IRule<ISelectedOccupation> GetOccupationRequiredRule(string key);
        IRule<long> GetAnnualIncomeRequiredRule(string key);

        IEnumerable<IRule<ResidencyStatus>> GetAustralianResidencyRules(string key, RiskProductDefinition riskProductDefinition);
        IEnumerable<IRule<DateTime>> GetMinimumAgeRules(string key, RiskProductDefinition riskProductDefinition);
        IEnumerable<IRule<DateTime>> GetMaximumAgeRules(string key, RiskProductDefinition riskProductDefinition);
        IEnumerable<IRule<long>> GetAnnualIncomeRules(string key, RiskProductDefinition riskProductDefinition);

        IRule<IEnumerable<IPlan>> GetAtLeastOnePlanMustBeSelectedRule(string key);
    }

    public class RiskRulesFactory : IRiskRulesFactory
    {
        public IRule<ResidencyStatus> GetResidencyStatusRequiredRule(string key)
        {
            return new ResidencyStatusRequiredRule(key);
        }


        public IRule<SmokerStatus> GetSmokerStatusRequiredRule(string key)
        {
            return new SmokerStatusRequiredRule(key);
        }

        public IRule<ISelectedOccupation> GetOccupationRequiredRule(string key)
        {
            return new OccupationRequiredRule(key);
        }

        public IRule<long> GetAnnualIncomeRequiredRule(string key)
        {
            return new AnnualIncomeRequiredRule(key);
        }

        public IEnumerable<IRule<ResidencyStatus>> GetAustralianResidencyRules(string key, RiskProductDefinition riskProductDefinition)
        {
            yield return new AustralianResidencyRule(riskProductDefinition, key);
        }

        public IEnumerable<IRule<DateTime>> GetMinimumAgeRules(string key, RiskProductDefinition riskProductDefinition)
        {
            yield return new MustBeOverMinumumAgeRule(riskProductDefinition.MinimumEntryAgeNextBirthday, key);
        }

        public IEnumerable<IRule<DateTime>> GetMaximumAgeRules(string key, RiskProductDefinition riskProductDefinition)
        {
            yield return new MustBeUnderMaxumumAgeRule(riskProductDefinition.MaximumEntryAgeNextBirthday, key); 
        }

        public IEnumerable<IRule<long>> GetAnnualIncomeRules(string key, RiskProductDefinition riskProductDefinition)
        {
            yield return new AnnualIncomeIsValidRule(riskProductDefinition, key);
        }

        public IRule<IEnumerable<IPlan>> GetAtLeastOnePlanMustBeSelectedRule(string key)
        {
            return new AtLeastOnePlanMustBeSelectedRule(key);
        }
    }
}

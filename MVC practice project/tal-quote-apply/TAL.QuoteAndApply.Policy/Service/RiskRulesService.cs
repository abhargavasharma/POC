using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IRiskRulesService
    {
        IEnumerable<RuleResult> ValidateRiskForInforce(RiskProductDefinition riskProductDefinition, IRisk risk);
    }

    public class RiskRulesService : IRiskRulesService
    {
        private readonly IRiskRulesFactory _riskRulesFactory;

        public RiskRulesService(IRiskRulesFactory riskRulesFactory)
        {
            _riskRulesFactory = riskRulesFactory;
        }

        private RuleResult GetRuleResult<TObj, TRuleType>(Func<string, IRule<TRuleType>> getRule,
             TObj obj, Expression<Func<TObj, TRuleType>> propertyExpression)
        {
            var propertyName = propertyExpression.GetName().ToCamelCase();
            var propertyValue = propertyExpression.GetValue(obj);
            return getRule.Invoke(propertyName).IsSatisfiedBy(propertyValue);
        }

        public IEnumerable<RuleResult> ValidateRiskForInforce(RiskProductDefinition riskProductDefinition, IRisk risk)
        {
            var ruleResults = new List<RuleResult>
            {
                GetRuleResult(_riskRulesFactory.GetResidencyStatusRequiredRule, risk, r => r.Residency),
                GetRuleResult(_riskRulesFactory.GetSmokerStatusRequiredRule, risk, r => r.SmokerStatus),
                _riskRulesFactory.GetOccupationRequiredRule("occupation").IsSatisfiedBy(risk),
                GetRuleResult(_riskRulesFactory.GetAnnualIncomeRequiredRule, risk, r => r.AnnualIncome)
            };

            ruleResults.Add(_riskRulesFactory.GetAustralianResidencyRules("residency", riskProductDefinition).AllAreSatisfied(risk.Residency));
            ruleResults.Add(_riskRulesFactory.GetMinimumAgeRules("dateOfBirth", riskProductDefinition).AllAreSatisfied(risk.DateOfBirth));
            ruleResults.Add(_riskRulesFactory.GetMaximumAgeRules("dateOfBirth", riskProductDefinition).AllAreSatisfied(risk.DateOfBirth));
            ruleResults.Add(_riskRulesFactory.GetAnnualIncomeRules("annualIncome", riskProductDefinition).AllAreSatisfied(risk.AnnualIncome));

            return ruleResults;
        }
    }
}
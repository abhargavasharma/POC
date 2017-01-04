using System;
using System.Collections;
using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Rules.Common;
using TAL.QuoteAndApply.Product.Service;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Product.Rules
{
    public interface IRuleFactory
    {
        IEnumerable<IRule<DateTime>> GetEligibleForPremiumTypeRules(PremiumTypeDefinition premiumTypeDefinition);
        IEnumerable<IRule<int>> GetMaxCoverAmountRules(ICoverAmountService coverAmountService, MaxCoverAmountParam maxCoverAmountParam);
        IEnumerable<IRule<int>> GetMinCoverAmountRules(PlanDefinition planDefinition);
        IEnumerable<IRule<int>> GetMaxGreaterThanMinCoverAmountRule(PlanDefinition planDefinition);
        IEnumerable<IRule<object>> GetFieldIsRequiredRule();
        IEnumerable<IRule<PremiumType>> GetPremiumTypeIsNotUnkownRule();
        IRule<decimal> GetMustHaveCoverAmountRule();
        IRule<IEnumerable<string>> GetSelectedPlanMustHaveAtLeastCoverSelected();
        IEnumerable<IRule<object>> GetVariableOptionMustBeValidRule(PlanVariablesDefinition variableDefinition);
    }

    public class RuleFactory : IRuleFactory
    {
        public IEnumerable<IRule<DateTime>> GetEligibleForPremiumTypeRules(PremiumTypeDefinition premiumTypeDefinition)
        {
            yield return new EligibleForPremiumTypeRule(premiumTypeDefinition);
        }
        
        public IEnumerable<IRule<int>> GetMaxCoverAmountRules(ICoverAmountService coverAmountService, MaxCoverAmountParam maxCoverAmountParam)
        {
            yield return new MustBeUnderMaxCoverAmountRule(coverAmountService, maxCoverAmountParam);
        }

        public IEnumerable<IRule<int>> GetMinCoverAmountRules(PlanDefinition planDefinition)
        {
            yield return new MustBeOverMinumumCoverAmountRule(planDefinition);
        }

        public IEnumerable<IRule<int>> GetMaxGreaterThanMinCoverAmountRule(PlanDefinition planDefinition)
        {
            yield return new MaxCoverAmountMustBeOverMinCoverAmount(planDefinition);
        }

        public IEnumerable<IRule<object>> GetFieldIsRequiredRule()
        {
            yield return new MustHaveFieldSelected();
        }

        public IEnumerable<IRule<PremiumType>> GetPremiumTypeIsNotUnkownRule()
        {
            yield return new PremiumTypeIsNotUnkownRule();
        }

        public IRule<decimal> GetMustHaveCoverAmountRule()
        {
            return new MustHaveCoverAmountRule();
        }

        public IRule<IEnumerable<string>> GetSelectedPlanMustHaveAtLeastCoverSelected()
        {
            return new SelectedPlanMustHaveAtLeastCoverSelected();
        }

        public IEnumerable<IRule<object>> GetVariableOptionMustBeValidRule(PlanVariablesDefinition variableDefinition)
        {
            yield return new SelectedVariableOptionMustBeValidOptionRule(variableDefinition);
        }
    }
}

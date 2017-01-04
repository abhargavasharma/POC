using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Plan;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules
{
    [TestFixture]
    public class OccupationDefinitionIsAllowedRuleTests
    {

        [Test]
        public void IsSatisfiedBy_RiskHasTpdAnyAndSatisfiedByAnyOccupation_True()
        {
            var riskDto = new RiskDto();
            riskDto.IsTpdAny = true;
            riskDto.IsTpdOwn = true;

            var rule = new OccupationDefinitionIsAllowedRule(riskDto);

            var ruleResult = rule.IsSatisfiedBy(OccupationDefinition.AnyOccupation);

            Assert.That(ruleResult.IsSatisfied, Is.True );
        }

        [Test]
        public void IsSatisfiedBy_RiskDoesntHaveTpdAnyAndSatisfiedByAnyOccupation_True()
        {
            var riskDto = new RiskDto();
            riskDto.IsTpdAny = false;
            riskDto.IsTpdOwn = true;

            var rule = new OccupationDefinitionIsAllowedRule(riskDto);

            var ruleResult = rule.IsSatisfiedBy(OccupationDefinition.AnyOccupation);

            Assert.That(ruleResult.IsSatisfied, Is.False);
        }

        [Test]
        public void IsSatisfiedBy_RiskHasTpdOwnAndSatisfiedByAnyOccupation_True()
        {
            var riskDto = new RiskDto();
            riskDto.IsTpdAny = true;
            riskDto.IsTpdOwn = true;

            var rule = new OccupationDefinitionIsAllowedRule(riskDto);

            var ruleResult = rule.IsSatisfiedBy(OccupationDefinition.OwnOccupation);

            Assert.That(ruleResult.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_RiskDoesntHaveTpdOwnAndSatisfiedByAnyOccupation_True()
        {
            var riskDto = new RiskDto();
            riskDto.IsTpdAny = true;
            riskDto.IsTpdOwn = false;

            var rule = new OccupationDefinitionIsAllowedRule(riskDto);

            var ruleResult = rule.IsSatisfiedBy(OccupationDefinition.OwnOccupation);

            Assert.That(ruleResult.IsSatisfied, Is.False);
        }

        [Test]
        public void IsSatisfiedBy_RiskHasNeitherAndSatisfiedByUnknown_True()
        {
            var riskDto = new RiskDto();
            riskDto.IsTpdAny = false;
            riskDto.IsTpdOwn = false;

            var rule = new OccupationDefinitionIsAllowedRule(riskDto);

            var ruleResult = rule.IsSatisfiedBy(OccupationDefinition.Unknown);

            Assert.That(ruleResult.IsSatisfied, Is.True);
        }
    }
}

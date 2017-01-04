using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules.Risk
{
    [TestFixture]
    public class AtLeastOnePlanMustBeSelectedRuleTests
    {
        [Test]
        public void IsSatisfied_NullPlans_IsBroken()
        {   
            var rule = new AtLeastOnePlanMustBeSelectedRule("TEST");
            var result = rule.IsSatisfiedBy(null);

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_NoPlans_IsBroken()
        {
            var rule = new AtLeastOnePlanMustBeSelectedRule("TEST");
            var result = rule.IsSatisfiedBy(new List<IPlan>());

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_NoSelectedPlans_IsBroken()
        {
            var rule = new AtLeastOnePlanMustBeSelectedRule("TEST");
            var result = rule.IsSatisfiedBy(new List<IPlan> {new PlanDto {Selected = false} });

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfied_OneSelectedPlan_IsSatisfied()
        {
            var rule = new AtLeastOnePlanMustBeSelectedRule("TEST");
            var result = rule.IsSatisfiedBy(new List<IPlan> { new PlanDto { Selected = true } });

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfied_OneSelectedPlanOneUnSelectedPlan_IsSatisfied()
        {
            var rule = new AtLeastOnePlanMustBeSelectedRule("TEST");
            var result = rule.IsSatisfiedBy(new List<IPlan> { new PlanDto { Selected = true }, new PlanDto { Selected = false } });

            Assert.That(result.IsSatisfied, Is.True);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TAL.QuoteAndApply.Rules.GenericRules;

namespace TAL.QuoteAndApply.Rules.UnitTests
{
    [TestFixture]
    public class ValidEmailAddressRuleTests
    {
        [Test]
        public void IsSatisfiedBy_TestWithSpaces_IsFalse()
        {
            var rule = new ValidEmailAddressRule("key");
            var result = rule.IsSatisfiedBy("chr s@gmail.com");
            
            Assert.That(result.IsSatisfied, Is.False);
        }

        [Test]
        public void IsSatisfiedBy_TestWithoutSpaces_IsTrue()
        {
            var rule = new ValidEmailAddressRule("key");
            var result = rule.IsSatisfiedBy("chris@gmail.com");

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_InvalidEMail_IsFalse()
        {
            var rule = new ValidEmailAddressRule("key");
            var result = rule.IsSatisfiedBy("@gmail.com");

            Assert.That(result.IsSatisfied, Is.False);
        }
    }
}

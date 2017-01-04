using System;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Rules.Common;

namespace TAL.QuoteAndApply.Product.UnitTests.Rules
{
    [TestFixture]
    public class EligibleForPremiumTypeRuleTests
    {

        [Test]
        public void IsSatisfiedBy_MaxAgeIsNull_IsSatisfied()
        {
            var premiumTypeDefinition = new PremiumTypeDefinition(PremiumType.Stepped, "Stepped", null);
            var premiumTypeEligibleRule = new EligibleForPremiumTypeRule(premiumTypeDefinition);
            var dateOfBirth = DateTime.Now.Date.AddYears(-21);

            var result = premiumTypeEligibleRule.IsSatisfiedBy(dateOfBirth);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [TestCase(59, 60)]
        [TestCase(60, 60)]
        [TestCase(61, 60)]
        public void IsSatisfiedBy_AgeNextBirthdayOverOrEqualToMaxAge_IsBroken(int ageNextBirthday, int? maxEntryAge)
        {
            var premiumTypeDefinition = new PremiumTypeDefinition(PremiumType.Stepped, "Stepped", maxEntryAge);
            var premiumTypeEligibleRule = new EligibleForPremiumTypeRule(premiumTypeDefinition);
            var dateOfBirth = DateTime.Now.Date.AddYears(-ageNextBirthday);

            var result = premiumTypeEligibleRule.IsSatisfiedBy(dateOfBirth);

            Assert.That(result.IsBroken, Is.True);
        }

        [TestCase(-1, 60)]
        [TestCase(0, 60)]
        [TestCase(58, 60)]
        public void IsSatisfiedBy_AgeNextBirthdayIsUnderMaxAge_IsSatisfied(int ageNextBirthday, int? maxEntryAge)
        {
            var premiumTypeDefinition = new PremiumTypeDefinition(PremiumType.Stepped, "Stepped", maxEntryAge);
            var premiumTypeEligibleRule = new EligibleForPremiumTypeRule(premiumTypeDefinition);
            var dateOfBirth = DateTime.Now.Date.AddYears(-ageNextBirthday);

            var result = premiumTypeEligibleRule.IsSatisfiedBy(dateOfBirth);

            Assert.That(result.IsSatisfied, Is.True);
        }

    }
}

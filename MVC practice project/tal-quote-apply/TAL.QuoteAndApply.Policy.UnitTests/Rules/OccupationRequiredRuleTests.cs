using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;

namespace TAL.QuoteAndApply.Policy.UnitTests.Rules
{
    [TestFixture]
    public class OccupationRequiredRuleTests
    {
        [Test]
        public void IsSatisfiedBy_AllOccupationFieldsComplete_IsSatisfied()
        {
            var occ = new MockSelectedOccupation()
            {
                OccupationClass = "Test",
                OccupationCode = "Test",
                OccupationTitle = "Test"
            };

            var occupationRule = new OccupationRequiredRule("");
            var result = occupationRule.IsSatisfiedBy(occ);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_OccupationClassEmpty_IsBroken()
        {
            var occ = new MockSelectedOccupation()
            {
                OccupationClass = "",
                OccupationCode = "Test",
                OccupationTitle = "Test"
            };

            var occupationRule = new OccupationRequiredRule("");
            var result = occupationRule.IsSatisfiedBy(occ);

            Assert.That(result.IsBroken, Is.False);
        }

        [Test]
        public void IsSatisfiedBy_OccupationCodeEmpty_IsBroken()
        {
            var occ = new MockSelectedOccupation()
            {
                OccupationClass = "Test",
                OccupationCode = "",
                OccupationTitle = "Test"
            };

            var occupationRule = new OccupationRequiredRule("");
            var result = occupationRule.IsSatisfiedBy(occ);

            Assert.That(result.IsBroken, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_OccupationTitleEmpty_IsBroken()
        {
            var occ = new MockSelectedOccupation()
            {
                OccupationClass = "Test",
                OccupationCode = "Test",
                OccupationTitle = ""
            };

            var occupationRule = new OccupationRequiredRule("");
            var result = occupationRule.IsSatisfiedBy(occ);

            Assert.That(result.IsBroken, Is.True);
        }

        public class MockSelectedOccupation : ISelectedOccupation {
            public string OccupationCode { get; set; }
            public string OccupationTitle { get; set; }
            public string OccupationClass { get; set; }
        }
    }
}
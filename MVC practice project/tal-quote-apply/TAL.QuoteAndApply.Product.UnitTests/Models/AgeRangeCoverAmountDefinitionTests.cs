using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.Product.UnitTests.Models
{
    [TestFixture]
    public class AgeRangeCoverAmountDefinitionTests
    {
        [Test]
        public void BuilderTest_MaxCoverRequired()
        {
            var builder = AgeRangeCoverAmountDefinition.Builder()
                .WithAgeRangeDefinition(10, 20);

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Test]
        public void BuilderTest_AgeRangeIsRequired()
        {
            var builder = AgeRangeCoverAmountDefinition.Builder()
                .WithMaxCover(100);

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Test]
        public void BuilderTest_IncomeFactorAdded()
        {
            var definition = AgeRangeCoverAmountDefinition.Builder()
                .WithAgeRangeDefinition(10, 20)
                .WithMaxCover(100)
                .WithAnnualIncomeFactor(10)
                .Build();

            Assert.That(definition.NoIncomeMaxCover, Is.Null);
            Assert.That(definition.MaxCoverAmount, Is.EqualTo(100));
            Assert.That(definition.AnnualIncomeFactor, Is.EqualTo(10));
        }
    }
}

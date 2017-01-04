using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.Product.Rules.Common;
using TAL.QuoteAndApply.Product.Service;

namespace TAL.QuoteAndApply.Product.UnitTests.Rules
{
    [TestFixture]
    public class MustBeUnderMaxCoverAmountRuleTests
    {
        [Test]
        public void IsSatisfiedBy_MaxCoverAmountIsZero_IsSatisfied()
        {
            var mockMaxCoverAmountParam = new MaxCoverAmountParam("TEST", "tal", 20, 100000, 500000, null);

            var mockCoverAmountService = new Mock<ICoverAmountService>(MockBehavior.Strict);
            mockCoverAmountService.Setup(call => call.GetMaxCover(mockMaxCoverAmountParam)).Returns(0);

            var rule = new MustBeUnderMaxCoverAmountRule(mockCoverAmountService.Object, mockMaxCoverAmountParam);

            var result = rule.IsSatisfiedBy(mockMaxCoverAmountParam.CoverAmount);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_MaxCoverAmountIs100000_CoverAmountIs99999_IsSatisfied()
        {
            var mockMaxCoverAmountParam = new MaxCoverAmountParam("TEST", "tal", 20, 100000, 99999, null);

            var mockCoverAmountService = new Mock<ICoverAmountService>(MockBehavior.Strict);
            mockCoverAmountService.Setup(call => call.GetMaxCover(mockMaxCoverAmountParam)).Returns(100000);

            var rule = new MustBeUnderMaxCoverAmountRule(mockCoverAmountService.Object, mockMaxCoverAmountParam);

            var result = rule.IsSatisfiedBy(mockMaxCoverAmountParam.CoverAmount);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_MaxCoverAmountIs100000_CoverAmountIs100000_IsSatisfied()
        {
            var mockMaxCoverAmountParam = new MaxCoverAmountParam("TEST", "tal", 20, 100000, 100000, null);

            var mockCoverAmountService = new Mock<ICoverAmountService>(MockBehavior.Strict);
            mockCoverAmountService.Setup(call => call.GetMaxCover(mockMaxCoverAmountParam)).Returns(100000);

            var rule = new MustBeUnderMaxCoverAmountRule(mockCoverAmountService.Object, mockMaxCoverAmountParam);

            var result = rule.IsSatisfiedBy(mockMaxCoverAmountParam.CoverAmount);

            Assert.That(result.IsSatisfied, Is.True);
        }

        [Test]
        public void IsSatisfiedBy_MaxCoverAmountIs100000_CoverAmountIs100001_IsBroken()
        {
            var mockMaxCoverAmountParam = new MaxCoverAmountParam("TEST", "tal", 20, 100000, 100001, null);

            var mockCoverAmountService = new Mock<ICoverAmountService>(MockBehavior.Strict);
            mockCoverAmountService.Setup(call => call.GetMaxCover(mockMaxCoverAmountParam)).Returns(100000);

            var rule = new MustBeUnderMaxCoverAmountRule(mockCoverAmountService.Object, mockMaxCoverAmountParam);

            var result = rule.IsSatisfiedBy(mockMaxCoverAmountParam.CoverAmount);

            Assert.That(result.IsBroken, Is.True);
        }
    }
}

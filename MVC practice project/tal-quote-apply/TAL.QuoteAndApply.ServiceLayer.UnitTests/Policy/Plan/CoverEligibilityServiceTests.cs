using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Cover;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Plan
{
    public class CoverEligibilityServiceTests
    {
        private MockRepository _mockRepo;
        private Mock<ICoverRulesFactory> _mockCoverRulesFactory;
        private Mock<IPlanErrorMessageService> _mockPlanErrorMessagesService;
        private Mock<IRule<ICover>> _passingRule;
        private Mock<IRule<ICover>> _failingRule;

        [TestFixtureSetUp]
        public void Setup()
        {
            _mockRepo = new MockRepository(MockBehavior.Strict);
            _mockCoverRulesFactory = _mockRepo.Create<ICoverRulesFactory>();
            _mockPlanErrorMessagesService = _mockRepo.Create<IPlanErrorMessageService>();
            _passingRule = _mockRepo.Create<IRule<ICover>>();
            _failingRule = _mockRepo.Create<IRule<ICover>>();

            _passingRule.Setup(call => call.IsSatisfiedBy(It.IsAny<ICover>())).Returns(new RuleResult(true));
            _failingRule.Setup(call => call.IsSatisfiedBy(It.IsAny<ICover>())).Returns(new RuleResult(false));
        }

        private ICoverEligibilityService GetService()
        {
            return new CoverEligibilityService(_mockCoverRulesFactory.Object, _mockPlanErrorMessagesService.Object);
        }

        [Test]
        public void GetCoverEligibilityResult_CoverNotDeclined_CoverIsAvailable()
        {
            //Arrange
            _mockCoverRulesFactory.Setup(call => call.GetCoverNotUnderwritingDeclinedRule()).Returns(_passingRule.Object);
            var cover = new CoverDto {Code = "Cover1"};

            var service = GetService();

            //Act
            var result = service.GetCoverEligibilityResult(cover);

            //Assert
            Assert.That(result.EligibleForCover);
            Assert.That(result.CoverCode, Is.EqualTo("Cover1"));
        }

        [Test]
        public void GetCoverEligibilityResult_CoverDeclined_CoverIsNotAvailable()
        {
            //Arrange
            _mockCoverRulesFactory.Setup(call => call.GetCoverNotUnderwritingDeclinedRule()).Returns(_failingRule.Object);
            _mockPlanErrorMessagesService.Setup(call => call.GetSelectedCoverUndwritingDeclinedMessage()).Returns("INELIGIBLE");
            var cover = new CoverDto { Code = "Cover1" };

            var service = GetService();

            //Act
            var result = service.GetCoverEligibilityResult(cover);

            //Assert
            Assert.That(result.EligibleForCover, Is.False);
            Assert.That(result.CoverCode, Is.EqualTo("Cover1"));
            Assert.That(result.IneligibleReasons.Any());
            Assert.That(result.IneligibleReasons.First(), Is.EqualTo("INELIGIBLE"));
        }
    }
}

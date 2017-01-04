using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Retrieval;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Rules;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models;
using TAL.QuoteAndApply.UserRoles.Customer;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy
{
    [TestFixture]
    public class PolicyRetrievalStatusProviderTests
    {
        private MockRepository _mockRepo = new MockRepository(MockBehavior.Strict);
        private Mock<IPolicyService> _mockPolicyService;
        private Mock<ICustomerAuthenticationService> _mockCustomerAuthenticationService;
        private Mock<IPolicyRetrievalRulesFactory> _mockRetrievalRules;
        private Mock<IRule<IPolicy>> _passingRule;
        private Mock<IRule<IPolicy>> _failingRule;
        private Mock<ICurrentProductBrandProvider> _mockCurrentProductBrandProvider;

        private PolicyDto _defaultMockPolicy;
        private ProductBrand _defaultMockBrand;

        private const string QuoteReference = "Q123456789";
        private const string Password = "abcd1234";

        [SetUp]
        public void Setup()
        {
            _mockRepo = new MockRepository(MockBehavior.Strict);
            _mockPolicyService = _mockRepo.Create<IPolicyService>();
            _mockCustomerAuthenticationService = _mockRepo.Create<ICustomerAuthenticationService>();
            _mockRetrievalRules = _mockRepo.Create<IPolicyRetrievalRulesFactory>();
            _mockCurrentProductBrandProvider = _mockRepo.Create<ICurrentProductBrandProvider>();

            _passingRule = new Mock<IRule<IPolicy>>();
            _passingRule.Setup(call => call.IsSatisfiedBy(It.IsAny<IPolicy>())).Returns(new RuleResult(true));

            _failingRule = new Mock<IRule<IPolicy>>();
            _failingRule.Setup(call => call.IsSatisfiedBy(It.IsAny<IPolicy>())).Returns(new RuleResult(false));

            _defaultMockPolicy = new PolicyDto {BrandKey = "TAL"};
            _defaultMockBrand = new ProductBrand(1, "TAL", 1);
        }


        [Test]
        public void GetFrom_InvalidPolicy_PolicyIncomplete_ReturnsNotSaved()
        {
            //Arrange
            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(QuoteReference)).Returns((PolicyDto)null);

            //Act
            var result = GetProvider().GetFrom(QuoteReference, Password);

            Assert.That(result, Is.EqualTo(PolicyRetrievalStatus.QuoteReferenceNotFound));
        }

        [Test]
        public void GetFrom_PolicyNotSaved_PolicyIncomplete_ReturnsNotSaved()
        {
            //Arrange
            _mockRetrievalRules.Setup(call => call.GetMustBeSavedRule()).Returns(_failingRule.Object);
            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(QuoteReference)).Returns(_defaultMockPolicy);
            _mockCurrentProductBrandProvider.Setup(call => call.GetCurrent()).Returns(_defaultMockBrand);
            //Act
            var result = GetProvider().GetFrom(QuoteReference, Password);

            Assert.That(result, Is.EqualTo(PolicyRetrievalStatus.NotSaved));
        }

        [Test]
        public void GetFrom_PolicySaved_PolicySubmitted_ReturnsAlreadySubmitted()
        {
            //Arrange
            _mockRetrievalRules.Setup(call => call.GetMustBeSavedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotSubmittedRule()).Returns(_failingRule.Object);
            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(QuoteReference)).Returns(_defaultMockPolicy);
            _mockCurrentProductBrandProvider.Setup(call => call.GetCurrent()).Returns(_defaultMockBrand);
            SetMockAuthenticationResult(CustomerResultStatus.Success);

            //Act
            var result = GetProvider().GetFrom(QuoteReference, Password);

            Assert.That(result, Is.EqualTo(PolicyRetrievalStatus.AlreadySubmitted));
        }

        [Test]
        public void GetFrom_PolicySaved_PolicyReferredToUnderwriter_ReturnsReferredToUnderwriter()
        {
            //Arrange
            _mockRetrievalRules.Setup(call => call.GetMustBeSavedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotSubmittedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotReferredToUnderwriterRule()).Returns(_failingRule.Object);
            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(QuoteReference)).Returns(_defaultMockPolicy);
            _mockCurrentProductBrandProvider.Setup(call => call.GetCurrent()).Returns(_defaultMockBrand);
            SetMockAuthenticationResult(CustomerResultStatus.Success);

            //Act
            var result = GetProvider().GetFrom(QuoteReference, Password);

            Assert.That(result, Is.EqualTo(PolicyRetrievalStatus.ReferredToUnderwriter));
        }

        [Test]
        public void GetFrom_PolicySavedAndInCompleteWithIncorrectPassword_ReturnsCorrectRetrievalStatus()
        {
            //Arrange
            _mockRetrievalRules.Setup(call => call.GetMustBeSavedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotSubmittedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotReferredToUnderwriterRule()).Returns(_passingRule.Object);
            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(QuoteReference)).Returns(_defaultMockPolicy);
            _mockCurrentProductBrandProvider.Setup(call => call.GetCurrent()).Returns(_defaultMockBrand);
            SetMockAuthenticationResult(CustomerResultStatus.Failure);

            //Act
            var result = GetProvider().GetFrom(QuoteReference, Password);

            Assert.That(result, Is.EqualTo(PolicyRetrievalStatus.InvalidPassword));
        }

        [Test]
        public void GetFrom_PolicySavedAndDifferentBrand_ReturnsCorrectRetrievalStatus()
        {
            //Arrange
            _mockRetrievalRules.Setup(call => call.GetMustBeSavedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotSubmittedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotReferredToUnderwriterRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotLockedOutRule()).Returns(_passingRule.Object);
            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(QuoteReference)).Returns(_defaultMockPolicy);
            _mockCurrentProductBrandProvider.Setup(call => call.GetCurrent()).Returns(new ProductBrand(1, "NotTAL", 1));
            SetMockAuthenticationResult(CustomerResultStatus.Success);

            //Act
            var result = GetProvider().GetFrom(QuoteReference, Password);

            Assert.That(result, Is.EqualTo(PolicyRetrievalStatus.InvalidBrand));
        }


        [Test]
        public void GetFrom_PolicySavedAndInCompleteWithCorrectPasswordWithBrandCaseSensitivityMismatch_ReturnsCorrectRetrievalStatus()
        {
            //Arrange
            _mockRetrievalRules.Setup(call => call.GetMustBeSavedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotSubmittedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotReferredToUnderwriterRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotLockedOutRule()).Returns(_passingRule.Object);
            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(QuoteReference)).Returns(new PolicyDto { BrandKey = "uppercase"});
            _mockCurrentProductBrandProvider.Setup(call => call.GetCurrent()).Returns(new ProductBrand(1, "UPPERCASE", 1));
            SetMockAuthenticationResult(CustomerResultStatus.Success);

            //Act
            var result = GetProvider().GetFrom(QuoteReference, Password);

            Assert.That(result, Is.EqualTo(PolicyRetrievalStatus.CanBeRetrieved));
        }


        [Test]
        public void GetFrom_PolicySavedAndInCompleteWithCorrectPasswordButIsLockedOut_ReturnsCorrectRetrievalStatus()
        {
            //Arrange
            _mockRetrievalRules.Setup(call => call.GetMustBeSavedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotSubmittedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotReferredToUnderwriterRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotLockedOutRule()).Returns(_failingRule.Object);
            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(QuoteReference)).Returns(_defaultMockPolicy);
            _mockCurrentProductBrandProvider.Setup(call => call.GetCurrent()).Returns(_defaultMockBrand);
            SetMockAuthenticationResult(CustomerResultStatus.Success);

            //Act
            var result = GetProvider().GetFrom(QuoteReference, Password);

            Assert.That(result, Is.EqualTo(PolicyRetrievalStatus.LockedOutDueToRefer));
        }

        [Test]
        public void GetFrom_PolicySavedAndInCompleteWithCorrectPassword_ReturnsCorrectRetrievalStatus()
        {
            //Arrange
            _mockRetrievalRules.Setup(call => call.GetMustBeSavedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotSubmittedRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotReferredToUnderwriterRule()).Returns(_passingRule.Object);
            _mockRetrievalRules.Setup(call => call.GetNotLockedOutRule()).Returns(_passingRule.Object);
            _mockPolicyService.Setup(call => call.GetByQuoteReferenceNumber(QuoteReference)).Returns(_defaultMockPolicy);
            _mockCurrentProductBrandProvider.Setup(call => call.GetCurrent()).Returns(_defaultMockBrand);
            SetMockAuthenticationResult(CustomerResultStatus.Success);

            //Act
            var result = GetProvider().GetFrom(QuoteReference, Password);

            Assert.That(result, Is.EqualTo(PolicyRetrievalStatus.CanBeRetrieved));
        }

        private void SetMockAuthenticationResult(CustomerResultStatus resultStatus)
        {
            var mockResult = new CustomerAuthenticateResult {Status = resultStatus};
            _mockCustomerAuthenticationService.Setup(call => call.Authenticate(QuoteReference, Password))
                .Returns(mockResult);
        }

        private IPolicyRetrievalStatusProvider GetProvider()
        {
            return new PolicyRetrievalStatusProvider(
                _mockPolicyService.Object,
                _mockCustomerAuthenticationService.Object,
                _mockRetrievalRules.Object,
                _mockCurrentProductBrandProvider.Object);
        }

    }
}

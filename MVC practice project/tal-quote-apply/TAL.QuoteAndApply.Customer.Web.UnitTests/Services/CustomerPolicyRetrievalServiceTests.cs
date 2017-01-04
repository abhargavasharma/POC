using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.UnitTests.Services
{
    [TestFixture]
    public class CustomerPolicyRetrievalServiceTests
    {
        private MockRepository _mockRepo = new MockRepository(MockBehavior.Strict);
        private Mock<IQuoteSessionContext> _mockQuoteSessionContext;
        private Mock<IPolicyRetrievalStatusProvider> _mockPolicyRetrievalStatusProvider;
        private Mock<IPolicyAutoUpdateService> _mockPolicyAutoUpdateService;
        private Mock<IPolicyInteractionService> _mockInteractionsService;


        [SetUp]
        public void Setup()
        {
            _mockRepo = new MockRepository(MockBehavior.Strict);
            _mockQuoteSessionContext = _mockRepo.Create<IQuoteSessionContext>();
            _mockPolicyRetrievalStatusProvider = _mockRepo.Create<IPolicyRetrievalStatusProvider>();
            _mockPolicyAutoUpdateService = _mockRepo.Create<IPolicyAutoUpdateService>();
            _mockInteractionsService = _mockRepo.Create<IPolicyInteractionService>();
        }

        private ICustomerPolicyRetrievalService GetService()
        {
            return new CustomerPolicyRetrievalService(
                _mockQuoteSessionContext.Object,
                _mockPolicyRetrievalStatusProvider.Object,
                _mockPolicyAutoUpdateService.Object,
                _mockInteractionsService.Object);
        }

        [Test]
        public void RetrieveQuote_WhenPolicyRetrievalStatusCanBeRetrieved_CanRetrieve()
        {
            const string quoteReference = "M123456789";
            const string password = "derp";

            _mockPolicyRetrievalStatusProvider.Setup(call => call.GetFrom(quoteReference, password))
                .Returns(PolicyRetrievalStatus.CanBeRetrieved);
            _mockQuoteSessionContext.Setup(call => call.Set(It.IsAny<string>()));
            _mockQuoteSessionContext.Setup(
                call => call.QuoteSession).Returns(new QuoteSession() { QuoteReference = quoteReference, SessionData = new SessionData() { CallBackRequested = false} });
            _mockPolicyAutoUpdateService.Setup(
                call => call.AutoUpdatePlansForEligibililityAndRecalculatePremium(quoteReference));
            _mockInteractionsService.Setup(call => call.PolicyRetrievedByCustomer(quoteReference));

            var result = GetService().RetrieveQuote(quoteReference, password);

            Assert.That(result.CanRetrieve, Is.True);
        }

        [TestCase(PolicyRetrievalStatus.NotSaved)]
        [TestCase(PolicyRetrievalStatus.AlreadySubmitted)]
        [TestCase(PolicyRetrievalStatus.InvalidPassword)]
        [TestCase(PolicyRetrievalStatus.ReferredToUnderwriter)]        
        [TestCase(PolicyRetrievalStatus.HasInvalidStatus)]
        [TestCase(PolicyRetrievalStatus.LockedOutDueToRefer)]
        [TestCase(PolicyRetrievalStatus.InvalidBrand)]
        public void RetrieveQuote_WhenUnretrievablePolicyStatus_CannotRetrieve(PolicyRetrievalStatus policyRetrievalStatus)
        {
            const string quoteReference = "M123456789";
            const string password = "derp";

            _mockPolicyRetrievalStatusProvider.Setup(call => call.GetFrom(quoteReference, password))
                .Returns(policyRetrievalStatus);

            var result = GetService().RetrieveQuote(quoteReference, password);

            Assert.That(result.CanRetrieve, Is.False);            
        }
    }
}

using System;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Interactions.Service;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Interactions
{
    [TestFixture]
    public class PolicyInteractionServiceTests
    {
        private Mock<IInteractionsService> _mockPolicyInteractionService;
        private Mock<IPolicyService> _mockPolicyService;
        private Mock<IPolicyInteractionsResultConverter> _mockPolicyInteractionsResultConverter;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepository = new MockRepository(MockBehavior.Default);
            _mockPolicyInteractionService = mockRepository.Create<IInteractionsService>();
            _mockPolicyService = mockRepository.Create<IPolicyService>();
            _mockPolicyInteractionsResultConverter = mockRepository.Create<IPolicyInteractionsResultConverter>();
        }

        private PolicyInteractionsService GetService()
        {
            return new PolicyInteractionsService(_mockPolicyInteractionService.Object,_mockPolicyService.Object, _mockPolicyInteractionsResultConverter.Object);
        }

        [Test]
        public void PolicyAccessed_WhenCalled_IsSuccessful()
        {
            //Arrange
            var svc = GetService();
            bool result = false;
            //Act
            try
            {
                svc.PolicyAccessed(123);
                result = true;
            }
            catch (Exception)
            {
                result = false;
                throw new Exception("Service call failed");
            }

            //Assert
            Assert.IsTrue(result);
        }
    }
}

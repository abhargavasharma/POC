using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy
{
    [TestFixture]
    public class PolicyPremiumFrequencyProviderTests
    {
        private Mock<IPolicyService> _policyService;
        private Mock<IProductDefinitionProvider> _productDefinitionProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            _policyService = mockRepository.Create<IPolicyService>();
            _productDefinitionProvider = mockRepository.Create<IProductDefinitionProvider>();
        }

        private PolicyPremiumFrequencyProvider GetService()
        {
            return new PolicyPremiumFrequencyProvider(_policyService.Object, _productDefinitionProvider.Object);
        }

        private PolicyDto GetPolicy()
        {
            var qrn = "1234567890";
            var mockPolicy = new PolicyDto
            {
                PremiumFrequency = PremiumFrequency.Quarterly,
                QuoteReference = qrn
            };

            return mockPolicy;
        }

        [Test]
        public void GetPremiumFrequency_PolicyRetrieved_PremiumFrequencyReturned()
        {
            var mockPolicy = GetPolicy();

            _policyService.Setup(call => call.GetByQuoteReferenceNumber(mockPolicy.QuoteReference)).Returns(mockPolicy);
            _productDefinitionProvider.Setup(call => call.GetAvailablePremiumFrequencies((string)null)).Returns(new List<string>());

            var svc = GetService();
            var result = svc.GetPremiumFrequency(mockPolicy.QuoteReference);

            Assert.That(result.PremiumFrequency, Is.EqualTo(mockPolicy.PremiumFrequency));
            Assert.That(result.QuoteReferenceNumber, Is.EqualTo(mockPolicy.QuoteReference));
        }

        [Test]
        public void GetPremiumFrequency_AvailableFrequenciesReturned()
        {
            var mockPolicy = GetPolicy();

            _policyService.Setup(call => call.GetByQuoteReferenceNumber(mockPolicy.QuoteReference)).Returns(mockPolicy);
            _productDefinitionProvider.Setup(call => call.GetAvailablePremiumFrequencies((string)null)).Returns(new List<string> { "Foo", "Bar"});

            var svc = GetService();
            var result = svc.GetPremiumFrequency(mockPolicy.QuoteReference);

            Assert.That(result.AvailableFrequencies.Count(), Is.EqualTo(2));
            Assert.That(result.AvailableFrequencies.Contains("Foo"));
            Assert.That(result.AvailableFrequencies.Contains("Bar"));
        }
    }
}

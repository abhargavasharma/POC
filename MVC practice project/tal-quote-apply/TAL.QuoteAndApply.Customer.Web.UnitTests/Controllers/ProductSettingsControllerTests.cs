using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Customer.Web.Controllers.Api;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Web.Shared.Session;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;

namespace TAL.QuoteAndApply.Customer.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ProductSettingsControllerTests
    {
        private Mock<IPolicyOverviewProvider> _policyOverviewProvider;
        private Mock<IProductDefinitionProvider> _productDefinitionProvider;
        private Mock<IQuoteSessionContext> _quoteSessionContext;

        [TestFixtureSetUp]
        protected void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);

            _quoteSessionContext = mockRepo.Create<IQuoteSessionContext>();
            _policyOverviewProvider = mockRepo.Create<IPolicyOverviewProvider>();
            _productDefinitionProvider = mockRepo.Create<IProductDefinitionProvider>();
        }

        [Test]
        public void GetMaximumNumberOfBeneficiaries_Test()
        {
            _productDefinitionProvider.Setup(call => call.GetProductDefinition("TAL")).Returns(
                new ProductDetailsResult { MaximumNumberOfBeneficiaries = 5 });

            var controller = new ProductSettingsController(_quoteSessionContext.Object,
                                                    _policyOverviewProvider.Object,
                                                    new TestCurrentProductBrandProvider(),
                                                    _productDefinitionProvider.Object);

            var result = controller.GetMaximumNumberOfBeneficiaries() as OkNegotiatedContentResult<int>;
            Assert.That(result.Content, Is.EqualTo(5));
        }

        [Test]
        public void GetPaymentOptionsForProduct_Test()
        {
            _productDefinitionProvider.Setup(call => call.GetProductDefinition("TAL")).Returns(
                new ProductDetailsResult
                {
                    IsDirectDebitAvailable = false,
                    IsSuperannuationAvailable = true,
                    AvailableCreditCardTypes = new[] { CreditCardType.Amex }
                });

            var controller = new ProductSettingsController(_quoteSessionContext.Object,
                                                    _policyOverviewProvider.Object,
                                                    new TestCurrentProductBrandProvider(),
                                                    _productDefinitionProvider.Object);

            var result = controller.GetPaymentOptionsForProduct();

            var okResult = result as OkNegotiatedContentResult<AvailablePaymentOptionsViewModel>;
            Assert.That(okResult.Content.IsDirectDebitAvailable, Is.EqualTo(false));
            Assert.That(okResult.Content.IsCreditCardAvailable, Is.EqualTo(true));
            Assert.That(okResult.Content.IsSuperAvailable, Is.EqualTo(true));
        }
    }
}

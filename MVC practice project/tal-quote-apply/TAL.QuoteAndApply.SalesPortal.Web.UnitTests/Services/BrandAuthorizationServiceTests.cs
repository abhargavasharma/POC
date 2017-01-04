using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.UnitTests.Services
{
    [TestFixture]
    public class BrandAuthorizationServiceTests
    {
        private Mock<ICurrentProductBrandProvider> _currentBrandProvider;
        private Mock<IProductBrandProvider> _productBrandProvider;
        private Mock<ISalesPortalSessionContext> _salesPortalSessionContext;

        [SetUp]
        public void SetUp()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _currentBrandProvider = mockRepo.Create<ICurrentProductBrandProvider>();
            _productBrandProvider = mockRepo.Create<IProductBrandProvider>();
            _salesPortalSessionContext = mockRepo.Create<ISalesPortalSessionContext>();
        }

        private BrandAuthorizationService GetService()
        {
            return new BrandAuthorizationService(_currentBrandProvider.Object, _productBrandProvider.Object, _salesPortalSessionContext.Object);
        }

        [TestCase("Brand1", "Brand1", new[] { Role.Agent }, true)]
        [TestCase("Brand1", "Brand2", new[] { Role.Agent }, false)]
        [TestCase("Brand1", "Brand1", new[] { Role.Underwriter }, true)]
        [TestCase("Brand1", "Brand2", new[] { Role.Underwriter }, true)]
        [TestCase("Brand1", "Brand1", new[] { Role.Agent, Role.Underwriter }, true)]
        [TestCase("Brand1", "Brand2", new[] { Role.Agent, Role.Underwriter }, true)]
        [TestCase("Brand1", "Brand2", new[] { Role.ReadOnly }, false)]
        public void CanAccess_PolicyResult_Tests(string policyBrand, string currentBrand, Role[] roles, bool canAccess)
        {
            var mockPolicy = new PolicyOverviewResult
            {
                Brand = policyBrand,
                QuoteReferenceNumber = "1111"
            };

            _currentBrandProvider.Setup(call => call.GetCurrent())
                .Returns(new ProductBrand(1, currentBrand, 1));

            _salesPortalSessionContext.Setup(call => call.SalesPortalSession)
                .Returns(new SalesPortalSession("user", roles, "user@mail.com", "U", "Ser", currentBrand));

            var service = GetService();

            Assert.That(service.CanAccess(mockPolicy), Is.EqualTo(canAccess));
        }

        [TestCase("Brand1", "Brand1", new[] { Role.Agent }, true)]
        [TestCase("Brand1", "Brand2", new[] { Role.Agent }, false)]
        [TestCase("Brand1", "Brand1", new[] { Role.Underwriter }, true)]
        [TestCase("Brand1", "Brand2", new[] { Role.Underwriter }, true)]
        [TestCase("Brand1", "Brand1", new[] { Role.Agent, Role.Underwriter }, true)]
        [TestCase("Brand1", "Brand2", new[] { Role.Agent, Role.Underwriter }, true)]
        [TestCase("Brand1", "Brand2", new[] { Role.ReadOnly }, false)]
        public void CanAccess_QuoteReference_Tests(string policyBrand, string currentBrand, Role[] roles, bool canAccess)
        {
            var quoteReference = "111";

            _productBrandProvider.Setup(call => call.GetBrandKeyForQuoteReferenceNumber(quoteReference))
                .Returns(policyBrand);

            _currentBrandProvider.Setup(call => call.GetCurrent())
                .Returns(new ProductBrand(1, currentBrand, 1));

            _salesPortalSessionContext.Setup(call => call.SalesPortalSession)
                .Returns(new SalesPortalSession("user", roles, "user@mail.com", "U", "Ser", currentBrand));

            var service = GetService();

            Assert.That(service.CanAccess(quoteReference), Is.EqualTo(canAccess));
        }
    }
}

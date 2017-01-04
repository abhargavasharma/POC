using System.Dynamic;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.UnitTests.Services
{
    [TestFixture]
    public class SalesPortalUiBrandingHelperTests
    {
        private Mock<IPolicyOverviewProvider> _mockPolicyOverviewProvider;
        private Mock<ISalesPortalSessionContext> _mockSalesPortalSessionContext;
        private SalesPortalSession _mockSalesPortalSession;

        [SetUp]
        public void Setup()
        {
            _mockPolicyOverviewProvider = new Mock<IPolicyOverviewProvider>(MockBehavior.Strict);
            _mockSalesPortalSessionContext = new Mock<ISalesPortalSessionContext>(MockBehavior.Strict);
            _mockSalesPortalSession = new SalesPortalSession("test user", new Role[0], "dummy@email", "Testy", "Testerton", "TestBrand");
        }

        private ISalesPortalUiBrandingHelper GetHelperInstance()
        {
            return new SalesPortalUiBrandingHelper(_mockPolicyOverviewProvider.Object, _mockSalesPortalSessionContext.Object);
        }


        [Test]
        public void SetLoggedInBrandUi_WhenCalledWithLoggedInBrand_SetsCorrectViewbagAttributes()
        {
            //Arrange
            _mockSalesPortalSessionContext.Setup(s => s.SalesPortalSession)
                .Returns(_mockSalesPortalSession);
            var helper = GetHelperInstance();
            dynamic viewbag = new ExpandoObject();

            //Act
            helper.SetLoggedInBrandUi(viewbag);

            //Assert
            Assert.That(viewbag.Brand, Is.EqualTo("testbrand"));
        }


        [Test]
        public void SetLoggedInBrandUi_WhenCalledWithNoLoggedInBrand_SetsCorrectDefaultViewbagAttributes()
        {
            //Arrange
            _mockSalesPortalSessionContext.Setup(s => s.SalesPortalSession)
                .Returns((SalesPortalSession)null);
            var helper = GetHelperInstance();
            dynamic viewbag = new ExpandoObject();

            //Act
            helper.SetLoggedInBrandUi(viewbag);

            //Assert
            Assert.That(viewbag.Brand, Is.EqualTo("tal"));
        }


        [Test]
        public void SetQuoteBrandUi_WhenCalledForPolicyBrand_SetsCorrectViewbagAttributes()
        {
            //Arrange
            const string quoteReference = "Q12345678";
            _mockPolicyOverviewProvider.Setup(p => p.GetFor(quoteReference))
                .Returns(new PolicyOverviewResult {Brand = "TAL"});

            var helper = GetHelperInstance();
            dynamic viewbag = new ExpandoObject();

            //Act
            helper.SetQuoteBrandUi(quoteReference, viewbag);

            //Assert
            Assert.That(viewbag.Brand, Is.EqualTo("tal"));
        }
    }
}

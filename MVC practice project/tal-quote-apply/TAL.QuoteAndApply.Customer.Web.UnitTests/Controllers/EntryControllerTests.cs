using System.Web.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.Customer.Web.Controllers.View;
using TAL.QuoteAndApply.Customer.Web.Converters;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared;

namespace TAL.QuoteAndApply.Customer.Web.UnitTests.Controllers
{
    [TestFixture]
    public class EntryControllerTests
    {
        private Mock<IPolicyInitialisationMetadataService> _mockPolicyMetaDataService;
        private Mock<IPolicyInitialisationMetadataProvider> _mockPolicyMetaDataProvider;
        private Mock<ILoggingService> _mockLoggingService;

        [TestFixtureSetUp]
        protected void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);

            _mockPolicyMetaDataService = mockRepo.Create<IPolicyInitialisationMetadataService>();
            _mockPolicyMetaDataProvider = mockRepo.Create<IPolicyInitialisationMetadataProvider>();
            _mockLoggingService = mockRepo.Create<ILoggingService>();
        }

        private EntryController GetController()
        {
            return new EntryController(_mockPolicyMetaDataService.Object, _mockPolicyMetaDataProvider.Object, _mockLoggingService.Object);
        }

        [Test]
        public void Index_POST_InvalidModel_RedirectToBasicInfo()
        {
            var model = new EntryPointViewModel();

            _mockLoggingService.Setup(call => call.Error(It.IsAny<string>()));

            var controller = GetController();
            ControllerModelValidation.ValidateModel(controller, model);

            var result = controller.Index(model);

            Assert.That(result, Is.TypeOf<RedirectToRouteResult>());

            var redirectResult = (RedirectToRouteResult)result;

            Assert.That(redirectResult.RouteValues["Controller"], Is.EqualTo("BasicInfo"));
        }

        [Test]
        public void Index_POST_ValidModel_PolicyInitialisationMetadataSetInSession_RedirectToBasicInfo()
        {
            var model = new EntryPointViewModel() {ContactId = "123456789", CalculatorResultsJson = new JObject(), CalculatorAssumptionsJson = new JObject() };
            var metaData = new PolicyInitialisationMetadata(model.ContactId, model.CalculatorResultsJson, model.CalculatorAssumptionsJson, false, false);

            _mockPolicyMetaDataProvider.Setup(call => call.GetPolicyInitialisationMetadata(model)).Returns(metaData);
            _mockPolicyMetaDataService.Setup(call => call.SetPolicyInitialisationMetadataForSession(metaData));

            var controller = GetController();

            var result = controller.Index(model);

            Assert.That(result, Is.TypeOf<RedirectToRouteResult>());

            var redirectResult = (RedirectToRouteResult)result;

            Assert.That(redirectResult.RouteValues["Controller"], Is.EqualTo("BasicInfo"));
        }

    }
}

using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Customer.Web.Controllers.Api;
using TAL.QuoteAndApply.Customer.Web.Converters;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Validation;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using TAL.QuoteAndApply.Web.Shared.Session;
using UrlHelper = System.Web.Http.Routing.UrlHelper;

namespace TAL.QuoteAndApply.Customer.Web.UnitTests
{
    [TestFixture]
    public class PolicyControllerTests
    {
        private Mock<ICreateQuoteService> _mockCreateQuoteService;
        private Mock<IQuoteSessionContext> _mockQuoteSessionContext;
        private Mock<IQuoteParamConverter> _mockQuoteParamConverter;
        private Mock<IPolicyInitialisationMetadataService> _mockPolicyInitialisationMetadataService;
        private Mock<IPolicyInitialisationMetadataProvider> _mockPolicyMetaDataProvider;
        private Mock<ICaptchaService> _mockCaptchaService;
        private Mock<IPostcodeService> _mockPostcodeService;
        private Mock<UrlHelper> _mockUrlHelper;

        [SetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _mockCreateQuoteService = mockRepo.Create<ICreateQuoteService>();
            _mockQuoteSessionContext = mockRepo.Create<IQuoteSessionContext>();
            _mockQuoteParamConverter = mockRepo.Create<IQuoteParamConverter>();
            _mockCreateQuoteService = mockRepo.Create<ICreateQuoteService>();
            _mockPolicyInitialisationMetadataService = mockRepo.Create<IPolicyInitialisationMetadataService>();
            _mockPolicyMetaDataProvider = mockRepo.Create<IPolicyInitialisationMetadataProvider>();
            _mockCaptchaService = mockRepo.Create<ICaptchaService>();
            _mockPostcodeService = mockRepo.Create<IPostcodeService>();
            _mockUrlHelper = mockRepo.Create<UrlHelper>();
        }

        private PolicyController GetController()
        {

            _mockUrlHelper.Setup(call => call.Route(It.IsAny<string>(), It.IsAny<object>())).Returns("http://link");
            _mockCaptchaService.Setup(call => call.Verify(It.IsAny<string>())).Returns(true);

            var controller = new PolicyController(_mockCreateQuoteService.Object, _mockQuoteSessionContext.Object,
                _mockQuoteParamConverter.Object, _mockPolicyInitialisationMetadataService.Object,
                _mockCaptchaService.Object, _mockPostcodeService.Object, _mockPolicyMetaDataProvider.Object)
            {
                Url = _mockUrlHelper.Object
            };

            return controller;
        }

        [Test]
        public void InitQuote_WhenCalled_ReturnsBlankBasicInfoViewModel()
        {
            var policyController = GetController();
            var initialQuoteResponse = policyController.InitQuote() as OkNegotiatedContentResult<BasicInfoViewModel>;

            Assert.That(initialQuoteResponse, Is.Not.Null);
            Assert.That(initialQuoteResponse.Content, Is.Not.Null);
            Assert.That(initialQuoteResponse.Content.DateOfBirth, Is.Null);
            Assert.That(initialQuoteResponse.Content.Gender, Is.Null);
            Assert.That(initialQuoteResponse.Content.IsSmoker, Is.Null);
            Assert.That(initialQuoteResponse.Content.AnnualIncome, Is.Null);
        }

        [Test]
        public void CreateQuote_WhenModelStateErrors_ReturnsInvalidModelStateActionResult()
        {
            var policyController = GetController();

            policyController.ModelState.AddModelError("ErrorKey", "Error Message");

            var response = policyController.CreateQuoteFromBasicInfo(new BasicInfoViewModel()) as InvalidModelStateActionResult;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ModelState.Count, Is.EqualTo(1));
            Assert.That(response.ModelState.First().Key, Is.EqualTo("ErrorKey"));
            Assert.That(response.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("Error Message"));
        }

        [Test]
        public void CreateQuote_WhenCreateQuoteHasMinAgeError_ReturnsInvalidModelStateActionResult()
        {
            var policyController = GetController();

            var createQuoteResult =
                new CreateQuoteResult(new List<ValidationError>
                {
                    new ValidationError(null, ValidationKey.MinimumAge, "Min Age", ValidationType.Error, null)
                });

            _mockQuoteParamConverter.Setup(c => c.FromBasicInfoViewModel(It.IsAny<BasicInfoViewModel>()))
                .Returns(new CreateQuoteParam(null, null, false, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            _mockCreateQuoteService.Setup(s => s.CreateQuote(It.IsAny<CreateQuoteParam>())).Returns(createQuoteResult);
            
            var response = policyController.CreateQuoteFromBasicInfo(new BasicInfoViewModel()) as InvalidModelStateActionResult;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ModelState.Count, Is.EqualTo(1));
            Assert.That(response.ModelState.First().Key, Is.EqualTo("basicInfoViewModel.DateOfBirth"));
            Assert.That(response.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("Min Age"));
        }

        [Test]
        public void CreateQuote_WhenCreateQuoteHasMaxAgeError_ReturnsInvalidModelStateActionResult()
        {
            var policyController = GetController();

            var createQuoteResult =
                new CreateQuoteResult(new List<ValidationError>
                {
                    new ValidationError(null, ValidationKey.MaximumAge, "Max Age", ValidationType.Error,null)
                });

            _mockQuoteParamConverter.Setup(c => c.FromBasicInfoViewModel(It.IsAny<BasicInfoViewModel>()))
                .Returns(new CreateQuoteParam(null, null, false, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            _mockCreateQuoteService.Setup(s => s.CreateQuote(It.IsAny<CreateQuoteParam>())).Returns(createQuoteResult);


            var response = policyController.CreateQuoteFromBasicInfo(new BasicInfoViewModel()) as InvalidModelStateActionResult;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ModelState.Count, Is.EqualTo(1));
            Assert.That(response.ModelState.First().Key, Is.EqualTo("basicInfoViewModel.DateOfBirth"));
            Assert.That(response.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("Max Age"));
        }

        [Test]
        public void CreateQuote_WhenCreateQuoteHasAnnualIncomeError_ReturnsInvalidModelStateActionResult()
        {
            var policyController = GetController();

            var createQuoteResult =
                new CreateQuoteResult(new List<ValidationError>
                {
                    new ValidationError(null, ValidationKey.AnnualIncome, "Annual Income", ValidationType.Error, null)
                });

            _mockQuoteParamConverter.Setup(c => c.FromBasicInfoViewModel(It.IsAny<BasicInfoViewModel>()))
                .Returns(new CreateQuoteParam(null, null, false, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            _mockCreateQuoteService.Setup(s => s.CreateQuote(It.IsAny<CreateQuoteParam>())).Returns(createQuoteResult);


            var response = policyController.CreateQuoteFromBasicInfo(new BasicInfoViewModel()) as InvalidModelStateActionResult;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ModelState.Count, Is.EqualTo(1));
            Assert.That(response.ModelState.First().Key, Is.EqualTo("basicInfoViewModel.AnnualIncome"));
            Assert.That(response.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("Annual Income"));
        }

        [Test]
        public void CreateQuote_WhenNoErrors_ReturnsRedirectResult()
        {
            var policyController = GetController();

            _mockQuoteParamConverter.Setup(c => c.FromBasicInfoViewModel(It.IsAny<BasicInfoViewModel>()))
                .Returns(new CreateQuoteParam(null, null, false, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            _mockCreateQuoteService.Setup(s => s.CreateQuote(It.IsAny<CreateQuoteParam>())).Returns(new CreateQuoteResult("QuoteRef", "UwId"));
            _mockQuoteSessionContext.Setup(c => c.Set(It.IsAny<string>()));

            var response = policyController.CreateQuoteFromBasicInfo(new BasicInfoViewModel()) as RedirectActionResult;

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Url, Is.EqualTo("http://link"));
        }

    }
}

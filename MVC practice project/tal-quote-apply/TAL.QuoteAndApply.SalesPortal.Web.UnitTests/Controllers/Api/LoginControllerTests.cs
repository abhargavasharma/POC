using System.Security.Principal;
using System.Web;
using System.Web.Http.Routing;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.Tests.Shared;
using TAL.QuoteAndApply.UserRoles.Configuration;
using TAL.QuoteAndApply.UserRoles.Models;
using TAL.QuoteAndApply.UserRoles.Services;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.SalesPortal.Web.UnitTests.Controllers.Api
{
    [TestFixture]
    public class LoginControllerTests
    {
        private Mock<HttpContextBase> _mockHttpContext;
        private Mock<IAuthenticationService> _mockAuthService;
        private Mock<ISalesPortalSessionContext> _mockSalesPortalSessionContext;
        private Mock<ISalesPortalSessionConverter> _mockSalesPortalSessionConverter;
        private Mock<IUserRolesConfigurationProvider> _mockUserRolesConfigurationProvider;
        private MockRepository _mockRepo;


        [TestFixtureSetUp]
        public void Setup()
        {
            _mockRepo = new MockRepository(MockBehavior.Strict);
            _mockAuthService = _mockRepo.Create<IAuthenticationService>();
            _mockHttpContext = _mockRepo.Create<HttpContextBase>();
            _mockSalesPortalSessionContext = _mockRepo.Create<ISalesPortalSessionContext>();
            _mockSalesPortalSessionConverter = _mockRepo.Create<ISalesPortalSessionConverter>();
            _mockUserRolesConfigurationProvider = _mockRepo.Create<IUserRolesConfigurationProvider>();
        }

        [Test]
        public void Post_ModelStateInvalid_InvalidModelStateResultReturned()
        {
            var loginRequest = new LoginRequest {UserName = null, Password = "test", UseWindowsAuth = false};

            var ctrl = GetController();

            //simulate model state validation
            ControllerModelValidation.ValidateModel(ctrl, loginRequest);

            var result = ctrl.Post(loginRequest);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<InvalidModelStateActionResult>());

            var invalidModelState = (InvalidModelStateActionResult) result;
            Assert.That(invalidModelState.ModelState.ContainsKey("UserName"), Is.True);
        }

        [Test]
        public void Post_UseWindowsAuth_NoRoles_InvalidModelStateReturned()
        {
            var user = new UserDetails()
            {
                Name = @"TOWER\test.user",
                EmailAddress = "test@test.com",
                GivenName = "Test",
                Surname = "Testsurname"
            }; 

            var mockPrincipal = _mockRepo.Create<IPrincipal>();
            var mockIdentity = _mockRepo.Create<IIdentity>();

            mockIdentity.Setup(call => call.Name).Returns(user.Name);
            mockPrincipal.Setup(call => call.Identity).Returns(mockIdentity.Object);
            _mockHttpContext.Setup(call => call.User).Returns(mockPrincipal.Object);

            var loginRequest = new LoginRequest { UserName = null, Password = null, UseWindowsAuth = true };

            var mockAuthResponse = AuthenticationResult.Failure(user, AuthenticationFailureReason.NoRoles);
            _mockAuthService.Setup(call => call.AuthenticateCurrentWindowsUser("test.user")).Returns(mockAuthResponse);

            var ctrl = GetController();
            var result = ctrl.Post(loginRequest);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<InvalidModelStateActionResult>());

            var invalidModelState = (InvalidModelStateActionResult)result;
            Assert.That(invalidModelState.ModelState.ContainsKey("loginRequest"), Is.True);
            Assert.That(invalidModelState.ModelState["loginRequest"].Errors[0].ErrorMessage, Is.EqualTo("The user requested for login does not have permission to access this system."));
        }

        [Test]
        public void Post_UseWindowsAuth_SuccessfulLogin()
        {
            var user = new UserDetails()
            {
                Name = @"TOWER\test.user",
                EmailAddress = "test@test.com",
                GivenName = "Test",
                Surname = "Testsurname"
            };
            const string redirectUrl = "/IAmLoggedIn";
            const string domain = "TOWER";

            var mockPrincipal = _mockRepo.Create<IPrincipal>();
            var mockIdentity = _mockRepo.Create<IIdentity>();

            mockIdentity.Setup(call => call.Name).Returns(user.Name);
            mockPrincipal.Setup(call => call.Identity).Returns(mockIdentity.Object);
            _mockHttpContext.Setup(call => call.User).Returns(mockPrincipal.Object);

            var loginRequest = new LoginRequest { UserName = null, Password = null, UseWindowsAuth = true };

            var mockAuthResponse = AuthenticationResult.Success(user, new [] {Role.Agent});
            _mockAuthService.Setup(call => call.AuthenticateCurrentWindowsUser("test.user")).Returns(mockAuthResponse);

            var mockUrlHelper = _mockRepo.Create<UrlHelper>();
            mockUrlHelper.Setup(call => call.Route(It.IsAny<string>(), It.IsAny<object>())).Returns(redirectUrl);

            var mockSalesPortalSession = new SalesPortalSession(mockAuthResponse.UserName, mockAuthResponse.Roles, mockAuthResponse.EmailAddress, mockAuthResponse.GivenName, mockAuthResponse.Surname, "TAL");
            _mockUserRolesConfigurationProvider.Setup(call => call.Domain).Returns(domain);
            _mockSalesPortalSessionConverter.Setup(call => call.From(mockAuthResponse, domain)).Returns(mockSalesPortalSession);
            _mockSalesPortalSessionContext.Setup(call => call.Set(mockSalesPortalSession));

            var ctrl = GetController();
            ctrl.Url = mockUrlHelper.Object;


            var result = ctrl.Post(loginRequest);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<RedirectActionResult>());

            var redirectResult = (RedirectActionResult)result;
            Assert.That(redirectResult.Url, Is.EqualTo(redirectUrl));
        }

        [Test]
        public void Post_UseCredentials_InvalidCredentials_InvalidModelStateReturned()
        {
            var user = new UserDetails()
            {
                Name = @"TOWER\test.user",
                EmailAddress = "test@test.com",
                GivenName = "Test",
                Surname = "Testsurname"
            }; 
            const string password = @"password";

            var loginRequest = new LoginRequest { UserName = user.Name, Password = password, UseWindowsAuth = false };

            var mockAuthResponse = AuthenticationResult.Failure(user, AuthenticationFailureReason.InvalidCredentials);
            _mockAuthService.Setup(call => call.AuthenticateUser(user.Name, password)).Returns(mockAuthResponse);

            var ctrl = GetController();
            var result = ctrl.Post(loginRequest);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<InvalidModelStateActionResult>());

            var invalidModelState = (InvalidModelStateActionResult)result;
            Assert.That(invalidModelState.ModelState.ContainsKey("loginRequest"), Is.True);
            Assert.That(invalidModelState.ModelState["loginRequest"].Errors[0].ErrorMessage, Is.EqualTo("Invalid user name or password. Please try again."));
        }

        [Test]
        public void Post_UseCredentials_NoRoles_InvalidModelStateReturned()
        {
            var user = new UserDetails()
            {
                Name = @"TOWER\test.user",
                EmailAddress = "test@test.com",
                GivenName = "Test",
                Surname = "Testsurname"
            };
            const string password = @"password";

            var loginRequest = new LoginRequest { UserName = user.Name, Password = password, UseWindowsAuth = false };

            var mockAuthResponse = AuthenticationResult.Failure(user, AuthenticationFailureReason.NoRoles);
            _mockAuthService.Setup(call => call.AuthenticateUser(user.Name, password)).Returns(mockAuthResponse);

            var ctrl = GetController();
            var result = ctrl.Post(loginRequest);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<InvalidModelStateActionResult>());

            var invalidModelState = (InvalidModelStateActionResult)result;
            Assert.That(invalidModelState.ModelState.ContainsKey("loginRequest"), Is.True);
            Assert.That(invalidModelState.ModelState["loginRequest"].Errors[0].ErrorMessage, Is.EqualTo("The user requested for login does not have permission to access this system."));
        }

        [Test]
        public void Post_UseCredentials_SuccessfulLogin()
        {
            var user = new UserDetails()
            {
                Name = @"TOWER\test.user",
                EmailAddress = "test@test.com",
                GivenName = "Test",
                Surname = "Testsurname"
            };
            const string password = @"password";
            const string redirectUrl = "/IAmLoggedIn";
            const string domain = "TOWER";

            var loginRequest = new LoginRequest { UserName = user.Name, Password = password, UseWindowsAuth = false };

            var mockAuthResponse = AuthenticationResult.Success(user, new[] {Role.Agent});
            _mockAuthService.Setup(call => call.AuthenticateUser(user.Name, password)).Returns(mockAuthResponse);

            var mockUrlHelper = _mockRepo.Create<UrlHelper>();
            mockUrlHelper.Setup(call => call.Route(It.IsAny<string>(), It.IsAny<object>())).Returns(redirectUrl);

            var mockSalesPortalSession = new SalesPortalSession(mockAuthResponse.UserName, mockAuthResponse.Roles, mockAuthResponse.EmailAddress, mockAuthResponse.GivenName, mockAuthResponse.Surname, "TAL");

            _mockUserRolesConfigurationProvider.Setup(call => call.Domain).Returns(domain);
            _mockSalesPortalSessionConverter.Setup(call => call.From(mockAuthResponse, domain)).Returns(mockSalesPortalSession);
            _mockSalesPortalSessionContext.Setup(call => call.Set(mockSalesPortalSession));

            var ctrl = GetController();
            ctrl.Url = mockUrlHelper.Object;

            var result = ctrl.Post(loginRequest);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<RedirectActionResult>());

            var redirectResult = (RedirectActionResult)result;
            Assert.That(redirectResult.Url, Is.EqualTo(redirectUrl));
        }

        private LoginController GetController()
        {
            return new LoginController(_mockAuthService.Object, 
                _mockHttpContext.Object, 
                _mockSalesPortalSessionContext.Object, 
                _mockSalesPortalSessionConverter.Object,
                _mockUserRolesConfigurationProvider.Object);
        }
    }
}

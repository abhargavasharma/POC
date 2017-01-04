using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.UserRoles.Configuration;
using TAL.QuoteAndApply.UserRoles.Models;
using TAL.QuoteAndApply.UserRoles.Services;
using TAL.QuoteAndApply.Web.Shared.HttpResults;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.Api
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly HttpContextBase _httpContextBase;
        private readonly ISalesPortalSessionContext _salesPortalSessionContext;
        private readonly ISalesPortalSessionConverter _salesPortalSessionConverter;
        private readonly IUserRolesConfigurationProvider _userRolesConfigurationProvider;

        public LoginController(IAuthenticationService authenticationService, 
            HttpContextBase httpContextBase, 
            ISalesPortalSessionContext salesPortalSessionContext,
            ISalesPortalSessionConverter salesPortalSessionConverter,
            IUserRolesConfigurationProvider userRolesConfigurationProvider)
        {
            _authenticationService = authenticationService;
            _httpContextBase = httpContextBase;
            _salesPortalSessionContext = salesPortalSessionContext;
            _salesPortalSessionConverter = salesPortalSessionConverter;
            _userRolesConfigurationProvider = userRolesConfigurationProvider;
        }

        [HttpPost]
        public IHttpActionResult Post(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return new InvalidModelStateActionResult(ModelState);
            }

            var authResult = AuthenticateUser(loginRequest);

            if (!authResult.Authenticated)
            {
                if (authResult.AuthenticationFailureReason == AuthenticationFailureReason.NoRoles)
                {
                    ModelState.AddModelError("loginRequest", "The user requested for login does not have permission to access this system.");
                }

                if (authResult.AuthenticationFailureReason == AuthenticationFailureReason.InvalidCredentials)
                {
                    ModelState.AddModelError("loginRequest", "Invalid user name or password. Please try again.");
                }

                return new InvalidModelStateActionResult(ModelState);
            }

            _salesPortalSessionContext.Set(_salesPortalSessionConverter.From(authResult, _userRolesConfigurationProvider.Domain));

            var redirectUrl = Url.Route("Default", new { Controller = "Home", Action = "PhoenixAuth" });

            return new RedirectActionResult(redirectUrl);
        }

        [HttpGet, Route("brands")]
        public IHttpActionResult Brands()
        {
            var brandsLoginModel = new BrandsLoginModel(_authenticationService.GetBrandsForCurrentUser(_salesPortalSessionContext.SalesPortalSession.UserName),
                _authenticationService.GetRolesForCurrentUser(_salesPortalSessionContext.SalesPortalSession.UserName).Contains(Role.Underwriter));
            return Ok(brandsLoginModel);
        }

        [WebApiSalesPortalSessionRequired]
        [HttpPost, Route("brand")]
        public IHttpActionResult SetSelectedBrand(SaveBrandRequest saveBrandRequest)
        {
            var currentSessionContext = _salesPortalSessionContext.SalesPortalSession;
            currentSessionContext.SelectedBrand = saveBrandRequest.Brand;
            _salesPortalSessionContext.Set(currentSessionContext);
            return Ok();
        }

        private AuthenticationResult AuthenticateUser(LoginRequest loginRequest)
        {
            AuthenticationResult authResult;
            if (loginRequest.UseWindowsAuth)
            {
                authResult = _authenticationService.AuthenticateCurrentWindowsUser(StripDomain(_httpContextBase.User.Identity.Name));
            }
            else
            {
                authResult = _authenticationService.AuthenticateUser(loginRequest.UserName, loginRequest.Password);
            }
            return authResult;
        }

        private string StripDomain(string userName)
        {
            var parts = userName.Split('\\');

            if (parts.Length > 1)
            {
                return parts[1];
            }

            return userName;
        }
    }
}

using System;
using System.Web.Mvc;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Attributes;
using TAL.QuoteAndApply.SalesPortal.Web.Configuration;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.SalesPortal.Web.Models.View;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.SalesPortal.Web.Services.Converters;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;

namespace TAL.QuoteAndApply.SalesPortal.Web.Controllers.View
{
    public class HomeController : BaseSalesPortalViewController
    {
        private readonly ISalesPortalSessionContext _salesPortalSessionContext;
        private readonly IUnderwritingUiAuthenticationService _underwritingUiAuthenticationService;
        private readonly IRoleDashboardRedirectService _roleDashboardRedirectService;
        private readonly IBrandExternalRefSettings _brandExternalRefSettings;

        public HomeController(ISalesPortalUiBrandingHelper brandingHelper, 
            ISalesPortalSessionContext salesPortalSessionContext,
            IUnderwritingUiAuthenticationService underwritingUiAuthenticationService, 
            IRoleDashboardRedirectService roleDashboardRedirectService,
            IBrandExternalRefSettings brandExternalRefSettings) : base(brandingHelper)
        {
            _salesPortalSessionContext = salesPortalSessionContext;
            _underwritingUiAuthenticationService = underwritingUiAuthenticationService;
            _roleDashboardRedirectService = roleDashboardRedirectService;
            _brandExternalRefSettings = brandExternalRefSettings;
        }

        [MvcSalesPortalSessionRequired]
        public ActionResult Index()
        {
            var redirectAction = _roleDashboardRedirectService.GetRedirectAction();
            var redirectController = _roleDashboardRedirectService.GetRedirectController();
            return RedirectToAction(redirectAction, redirectController);
        }

        public ActionResult Login(bool? timeout)
        {
            return View(new LoginViewModel {SessionTimeout = timeout.GetValueOrDefault(false)});
        }

        [MvcSalesPortalSessionRequired]
        public ActionResult PhoenixAuth()
        {
            var uri = new UriBuilder(Request.Url.AbsoluteUri)
            {
                Path = Url.Action(_roleDashboardRedirectService.GetRedirectAction(), _roleDashboardRedirectService.GetRedirectController())
            };

            var model = new PhoenixAuthViewModel()
            {
                AuthenticationToken = _underwritingUiAuthenticationService.GetAuthenticationTokenForCurrentUser(),
                CallBackOnSuccessUrl = uri.ToString(),
                CallBackOnErrorUrl = uri.ToString(),
                PhoenixUrl = _underwritingUiAuthenticationService.GetAuthenticationEndPoint()
            };
            return View(model);
        }

        [MvcSalesPortalSessionRequired]
        public ActionResult Logout()
        {
            _salesPortalSessionContext.Clear();
            return RedirectToAction("Login");
        }

        [MvcSalesPortalSessionRequired]
        public ActionResult Dashboard()
        {
            return RedirectToAction(_roleDashboardRedirectService.GetRedirectAction());
        }

        [MvcSalesPortalSessionRequired]
        public ActionResult UnderwriterDashboard()
        {
            var applicationCurrentUserProvider = DependencyResolver.Current.GetService<ICurrentUserProvider>();
            var user = applicationCurrentUserProvider.GetForApplication();
            return View(user);
        }
        
        [MvcSalesPortalSessionRequired]
        public ActionResult AgentDashboard()
        {
            var applicationCurrentUserProvider = DependencyResolver.Current.GetService<ICurrentUserProvider>();
            var externalRefDetails = _brandExternalRefSettings.ExternalCustomerRefSettings();

            var model = new AgentDashboardViewModel()
            {
                User = applicationCurrentUserProvider.GetForApplication(),
                BrandSettingsViewModel = externalRefDetails
            };
            return View(model);
        }
    }
}
using System;
using System.Linq;
using TAL.QuoteAndApply.DataModel.User;
using System.Web.Mvc;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public interface IRoleDashboardRedirectService
    {
        string GetRedirectAction();
        string GetRedirectController();
    }

    public class RoleDashboardRedirectService : IRoleDashboardRedirectService
    {
        private readonly ICurrentUserProvider _currentUserProvider;

        public RoleDashboardRedirectService(ICurrentUserProvider currentUserProvider)
        {
            _currentUserProvider = currentUserProvider;
        }

        public string GetRedirectAction()
        {
            var uriString = "Index";
            var user = _currentUserProvider.GetForApplication();
            if (user.Roles.Contains(Role.ReadOnly))
            {
                uriString = "Search";
            }
            if (user.Roles.Contains(Role.Agent))
            {
                uriString = "AgentDashboard";
            }
            if (user.Roles.Contains(Role.Underwriter))
            {
                uriString = "UnderwriterDashboard";
            }

            return uriString;
        }

        public string GetRedirectController()
        {
            var uriString = "Home";
            var user = _currentUserProvider.GetForApplication();
            if (user.Roles.Contains(Role.ReadOnly))
            {
                uriString = "Client";
            }
            if (user.Roles.Contains(Role.Agent))
            {
                uriString = "Home";
            }
            if (user.Roles.Contains(Role.Underwriter))
            {
                uriString = "Home";
            }

            return uriString;
        }
    }
}
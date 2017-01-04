using System.Linq;
using TAL.QuoteAndApply.SalesPortal.Web.User;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    public class SalesPortalIntegrationTestSessionContext : ISalesPortalSessionContext
    {
        private SalesPortalSession _salesPortalSession;

        public SalesPortalSession SalesPortalSession
        {
            get { return _salesPortalSession; }
        }

        public void Set(SalesPortalSession salesPortalSession)
        {
            _salesPortalSession = salesPortalSession;
        }

        public void Clear()
        {
            _salesPortalSession = null;
        }

        public void ExtendSession()
        {
            //I don't always Extend Sessions, but when I do, I don't do anything
        }

        public bool HasValue()
        {
            var sessionValue = SalesPortalSession;
            return sessionValue != null && !string.IsNullOrEmpty(sessionValue.UserName) && sessionValue.Roles.Any();
        }
    }
}
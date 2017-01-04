using System.Configuration;
using TAL.QuoteAndApply.UserRoles.Configuration;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public class UserRolesConfigurationProvider : IUserRolesConfigurationProvider
    {
        public string Domain
        {
            get { return ConfigurationManager.AppSettings["UserRoles.Domain"]; }
        }
    }
}
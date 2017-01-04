using TAL.QuoteAndApply.UserRoles.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.User
{
    public interface ISalesPortalSessionConverter
    {
        SalesPortalSession From(AuthenticationResult authenticationResult, string domain);
    }

    public class SalesPortalSessionConverter : ISalesPortalSessionConverter
    {
        public SalesPortalSession From(AuthenticationResult authenticationResult, string domain)
        {
            return new SalesPortalSession($@"{domain}\{authenticationResult.UserName}", authenticationResult.Roles, authenticationResult.EmailAddress, authenticationResult.GivenName, authenticationResult.Surname, authenticationResult.CurrentBrand);
        }
    }
}
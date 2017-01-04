using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.ServiceLayer.User;

namespace TAL.QuoteAndApply.Customer.Web.User
{
    public class CustomerPortalCurrentUser : IApplicationCurrentUser
    {
        public string CurrentUser
        {
            get
            {
                //todo: what do we do here?
                return "Customer";
            }
        }

        public IEnumerable<Role> CurrentUserRoles
        {
            get { return null; }
        }

        public string CurrentUserEmailAddress
        {
            get
            {
                return "Customer@email.com";
            }
        }

        public string CurrentUserGivenName
        {
            get
            {
                return "CustomerFirstName";
            }
        }
        public string CurrentUserSurname {
            get
            {
                return "CustomerSurname";
            }
        }
    }
}
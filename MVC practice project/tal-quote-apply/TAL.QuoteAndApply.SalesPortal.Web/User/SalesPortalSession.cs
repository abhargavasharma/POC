using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.UserRoles.Services;

namespace TAL.QuoteAndApply.SalesPortal.Web.User
{
    public class SalesPortalSession
    {
        public string UserName { get; }
        public IEnumerable<Role> Roles { get; }
        public string EmailAddress { get; }
        public string GivenName { get; }
        public string Surname { get; }
        public string SelectedBrand { get; set; }

        public SalesPortalSession(string userName, IEnumerable<Role> roles, string emailAddress, string givenName, string surname, string selectedBrand)
        {
            UserName = userName;
            Roles = roles;
            EmailAddress = emailAddress;
            GivenName = givenName;
            Surname = surname;
            SelectedBrand = selectedBrand;
        }
    }
}
using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;

namespace TAL.QuoteAndApply.ServiceLayer.User
{
    public class CurrentUser : ICurrentUser
    {
        public string UserName { get; }
        public IEnumerable<Role> Roles { get; }
        public string EmailAddress { get; }
        public string GivenName { get; }
        public string Surname { get; }

        public CurrentUser(string userName, IEnumerable<Role> roles, string emailAddress, string givenName, string surname)
        {
            UserName = userName;
            Roles = roles;
            GivenName = givenName;
            Surname = surname;
            EmailAddress = emailAddress;
        }
    }
}

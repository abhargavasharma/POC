using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.UserRoles.Services;

namespace TAL.QuoteAndApply.UserRoles.Models
{
    public enum AuthenticationFailureReason
    {
        NoRoles,
        InvalidCredentials
    }

    public class AuthenticationResult
    {
        public bool Authenticated { get; }
        public AuthenticationFailureReason? AuthenticationFailureReason { get; }
        public string UserName { get; }
        public IEnumerable<Role> Roles { get; }
        public string EmailAddress { get; }
        public string GivenName { get; }
        public string Surname { get; }
        public string CurrentBrand { get; }

        private AuthenticationResult(bool authenticated, AuthenticationFailureReason? authenticationFailureReason, UserDetails userDetails, IEnumerable<Role> roles)
        {
            Authenticated = authenticated;
            UserName = userDetails?.Name;
            EmailAddress = userDetails?.EmailAddress;
            GivenName = userDetails?.GivenName;
            Surname = userDetails?.Surname;
            Roles = roles;
            AuthenticationFailureReason = authenticationFailureReason;
        }

        public static AuthenticationResult Success(UserDetails userDetails, IEnumerable<Role> roles)
        {
            return new AuthenticationResult(true, null, userDetails, roles);
        }

        public static AuthenticationResult Failure(UserDetails userDetails, AuthenticationFailureReason authenticationFailureReason)
        {
            return new AuthenticationResult(false, authenticationFailureReason, userDetails, null);
        }
    }
}

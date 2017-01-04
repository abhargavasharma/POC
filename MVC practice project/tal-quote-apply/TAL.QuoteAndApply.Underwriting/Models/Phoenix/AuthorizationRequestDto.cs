using System;

namespace TAL.QuoteAndApply.Underwriting.Models.Phoenix
{
    public class AuthenticationRequestDto
    {
        public AuthenticationRequestDto(string userName)
        {
            UserName = userName;
        }

        public string UserName { get; private set; }
    }

    public class AuthorizationRequestDto
    {
        public AuthorizationRequestDto(string userName, AccessPermission[] permissions, DateTime tokenExpiry)
        {
            UserName = userName;
            Permissions = permissions;
            TokenExpiry = tokenExpiry;
        }

        public string UserName { get; private set; }
        public AccessPermission[] Permissions { get; private set; }
        public DateTime TokenExpiry { get; private set; }
    }
}
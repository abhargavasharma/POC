using System;

namespace TAL.QuoteAndApply.Underwriting.Models.Phoenix
{
    public class AuthenticationResponseDto
    {
        public AuthenticationResponseDto(string authenitcationToken)
        {
            AuthenitcationToken = authenitcationToken;
        }

        public string AuthenitcationToken { get; private set; }
    }

    public class AuthorisationResponseDto
    {
        public AuthorisationResponseDto(string token, string userName, AccessPermission[] permissions,
            string interviewId, DateTime tokenExpiry)
        {
            Token = token;
            UserName = userName;
            Permissions = permissions;
            InterviewId = interviewId;
            TokenExpiry = tokenExpiry;
        }

        public string Token { get; private set; }
        public string UserName { get; private set; }
        public string InterviewId { get; private set; }
        public DateTime TokenExpiry { get; private set; }
        public AccessPermission[] Permissions { get; private set; }
    }
}
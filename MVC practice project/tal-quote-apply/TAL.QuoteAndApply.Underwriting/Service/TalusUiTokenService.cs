using System;
using System.IO;
using System.Net;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Underwriting.Configuration;
using TAL.QuoteAndApply.Underwriting.Models.Phoenix;

namespace TAL.QuoteAndApply.Underwriting.Service
{
    public interface ITalusUiTokenService
    {
        AuthenticationResponseDto GetAuthenticationToken(string username);
        AuthorisationResponseDto GetAuthorisationToken(string interviewId, string username, AccessPermission[] permissions, DateTime tokenExpiry);
        AccessPermission[] GetAgentPermissions();
        AccessPermission[] GetReadOnlyPermissions();
        AccessPermission[] GetUnderwriterPermissions();
        AccessPermission[] GetUnderwriterReadOnlyPermissions();
    }

    public class TalusUiTokenService : ITalusUiTokenService
    {
        private readonly IHttpClientService _clientService;
        private readonly IUnderwritingConfigurationProvider _underwritingConfigurationProvider;

        public TalusUiTokenService(IHttpClientService clientService, IUnderwritingConfigurationProvider underwritingConfigurationProvider)
        {
            _clientService = clientService;
            _underwritingConfigurationProvider = underwritingConfigurationProvider;
        }

        public AuthenticationResponseDto GetAuthenticationToken(string username)
        {
            var request = new AuthenticationRequestDto(username);
            var authPath = "api/authentication";
            var authUrl = Path.Combine(_underwritingConfigurationProvider.TalusUiBaseUrl, authPath);

            var postRequest = new PutOrPostRequest(new Uri(authUrl), request)
                .WithCredentials(CredentialCache.DefaultNetworkCredentials);

            return _clientService.PostAsync<AuthenticationResponseDto>(postRequest).Result;
        }

        public AuthorisationResponseDto GetAuthorisationToken(string interviewId, string username, AccessPermission[] permissions, DateTime tokenExpiry)
        {
            var request = new AuthorizationRequestDto(username, permissions, tokenExpiry);
            var authPath = string.Format("api/authorisation/interview/{0}", interviewId);
            var authUrl = Path.Combine(_underwritingConfigurationProvider.TalusUiBaseUrl, authPath);

            var postRequest = new PutOrPostRequest(new Uri(authUrl), request)
                .WithCredentials(CredentialCache.DefaultNetworkCredentials);

            return _clientService.PostAsync<AuthorisationResponseDto>(postRequest).Result;
        }

        public AccessPermission[] GetAgentPermissions()
        {
            return new[] {AccessPermission.AnswerQuestions, AccessPermission.DisclosureNotes, AccessPermission.UnderwritingNotes };
        }

        public AccessPermission[] GetReadOnlyPermissions()
        {
            return new[] { AccessPermission.ReadOnly, AccessPermission.DisclosureNotesReadOnly, AccessPermission.UnderwritingNotesReadOnly };
        }

        public AccessPermission[] GetUnderwriterPermissions()
        {
            return new[] { AccessPermission.Override, AccessPermission.ReferralNotes, AccessPermission.UnderwritingNotes, AccessPermission.DisclosureNotesReadOnly };
        }

        public AccessPermission[] GetUnderwriterReadOnlyPermissions()
        {
            return new[] { AccessPermission.UnderwritingNotesReadOnly, AccessPermission.ReferralNotesReadOnly, AccessPermission.DisclosureNotesReadOnly };
        }
    }
}

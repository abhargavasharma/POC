using System.IO;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface IUnderwritingUiAuthenticationService
    {
        string GetAuthenticationTokenForCurrentUser();
        string GetAuthenticationEndPoint();
    }

    public class UnderwritingUiAuthenticationService : IUnderwritingUiAuthenticationService
    {
        private readonly ITalusUiTokenService _talusUiTokenService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUnderwritingConfiguration _underwritingConfiguration;

        public UnderwritingUiAuthenticationService(ITalusUiTokenService talusUiTokenService,
            ICurrentUserProvider currentUserProvider, IUnderwritingConfiguration underwritingConfiguration)
        {
            _talusUiTokenService = talusUiTokenService;
            _currentUserProvider = currentUserProvider;
            _underwritingConfiguration = underwritingConfiguration;
        }

        public string GetAuthenticationTokenForCurrentUser()
        {
            var currentUser = _currentUserProvider.GetForApplication().UserName;
            var tokenResponse = _talusUiTokenService.GetAuthenticationToken(currentUser);
            return tokenResponse.AuthenitcationToken;
        }

        public string GetAuthenticationEndPoint()
        {
            return Path.Combine(_underwritingConfiguration.TalusUiBaseUrl, "api/login/authenticate");
        }
    }
}
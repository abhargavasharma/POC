using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Underwriting.Models.Phoenix;
using TAL.QuoteAndApply.Underwriting.Service;
using TAL.QuoteAndApply.UserRoles.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface ITalusUiUrlService
    {
        string GetTalusUiUrlWithPermissionsFor(string quoteReference, int riskId);
    }

    public class TalusUiUrlService : ITalusUiUrlService
    {
        private readonly IPolicyService _policyService;
        private readonly IRiskService _riskService;
        private readonly ITalusUiTokenService _talusUiTokenService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnderwritingConfiguration _underwritingConfiguration;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUrlUtilities _urlUtilities;

        public TalusUiUrlService(IPolicyService policyService, IRiskService riskService, ITalusUiTokenService talusUiTokenService, 
            IDateTimeProvider dateTimeProvider, IUnderwritingConfiguration underwritingConfiguration, ICurrentUserProvider currentUserProvider,
            IUrlUtilities urlUtilities)
        {
            _riskService = riskService;
            _talusUiTokenService = talusUiTokenService;
            _dateTimeProvider = dateTimeProvider;
            _underwritingConfiguration = underwritingConfiguration;
            _currentUserProvider = currentUserProvider;
            _urlUtilities = urlUtilities;
            _policyService = policyService;
        }

        public string GetTalusUiUrlWithPermissionsFor(string quoteReference, int riskId)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);
            var risk = _riskService.GetRisk(riskId);

            var user = _currentUserProvider.GetForApplication();

            if (policy.Status == PolicyStatus.Incomplete)
            {
                if (user.Roles.Contains(Role.Agent))
                {
                    return GetAgentTalusUiUrl(risk, user);
                }
            }

            if (policy.Status == PolicyStatus.ReferredToUnderwriter)
            {
                if (user.Roles.Contains(Role.Underwriter))
                {
                    return GetUnderwriterTalusUiUrl(risk, user);
                }
            }

            if (user.Roles.Contains(Role.Underwriter))
            {
                return GetUnderwriterReadonlyTalusUiUrl(risk, user);
            }

            return GetReadonlyTalusUiUrl(risk, user);
        }

        private string GetAgentTalusUiUrl(IRisk risk, ICurrentUser currentUser)
        {
            var permissions = _talusUiTokenService.GetAgentPermissions();
            return GetUrl(risk, currentUser, permissions);
        }

        private string GetUnderwriterTalusUiUrl(IRisk risk, ICurrentUser currentUser)
        {
            var permissions = _talusUiTokenService.GetUnderwriterPermissions();
            return GetUrl(risk, currentUser, permissions);
        }

        private string GetReadonlyTalusUiUrl(IRisk risk, ICurrentUser currentUser)
        {
            var permissions = _talusUiTokenService.GetReadOnlyPermissions();
            return GetUrl(risk, currentUser, permissions);
        }

        private string GetUnderwriterReadonlyTalusUiUrl(IRisk risk, ICurrentUser currentUser)
        {
            var permissions = _talusUiTokenService.GetUnderwriterReadOnlyPermissions();
            return GetUrl(risk, currentUser, permissions);
        }

        private string GetUrl(IRisk risk, ICurrentUser currentUser, AccessPermission[] accessPermissions)
        {
            var etagQueryString = GetEtagQueryString(risk);

            var authTokenContainer = _talusUiTokenService.GetAuthorisationToken(risk.InterviewId, currentUser.UserName, accessPermissions, _dateTimeProvider.GetCurrentDate().AddDays(1));

            return $"{_underwritingConfiguration.TalusUiBaseUrl}/#/token/{authTokenContainer.Token}{etagQueryString}";
        }


    private string GetEtagQueryString(IRisk risk)
        {
            var etagQueryString = string.Empty;
            if (risk.InterviewConcurrencyToken != null)
            {
                var encodedConcurrencyToken = _urlUtilities.UrlEncode(risk.InterviewConcurrencyToken);
                etagQueryString = $"?etag={encodedConcurrencyToken}";
            }
            return etagQueryString;
        }
        
    }
}

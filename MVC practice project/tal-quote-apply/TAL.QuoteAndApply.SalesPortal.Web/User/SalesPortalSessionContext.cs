using System;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.Web.Shared.Cookie;

namespace TAL.QuoteAndApply.SalesPortal.Web.User
{
    public interface ISalesPortalSessionContext
    {
        void Set(SalesPortalSession salesPortalSession);
        SalesPortalSession SalesPortalSession { get; }
        void Clear();
        bool HasValue();
        void ExtendSession();
    }

    public class SalesPortalSessionContext : ISalesPortalSessionContext
    {
        private readonly ISalesPortalConfiguration _salesPortalConfiguration;
        private readonly ISecurityService _securityService;
        private readonly ICookieService _cookieService;
        private readonly ILoggingService _loggingService;

        private const string CookieName = "tL2Taa1aoSLAt1";

        public SalesPortalSessionContext(
            ISalesPortalConfiguration salesPortalConfiguration,
            ISecurityService securityService,
            ICookieService cookieService, 
            ILoggingService loggingService
            )
        {
            _salesPortalConfiguration = salesPortalConfiguration;
            _securityService = securityService;
            _cookieService = cookieService;
            _loggingService = loggingService;
        }

        public void Set(SalesPortalSession salesPortalSession)
        {
            Random generator = new Random();
            var randomString = generator.Next(0, 1000000).ToString("D6");

            var stringToEncrypt = $"{randomString}|{salesPortalSession.ToJson()}";

            var timeout = _salesPortalConfiguration.SessionTimeout;

            _cookieService.SetCookie(CookieName, _securityService.Encrypt(stringToEncrypt), DateTime.Now.AddMinutes(timeout));
        }

        public SalesPortalSession SalesPortalSession
        {
            get
            {
                var cookieValue = _cookieService.GetCookieValue(CookieName);

                if (!string.IsNullOrEmpty(cookieValue))
                {
                    try
                    {
                        var value = _securityService.Decrypt(cookieValue);

                        if (string.IsNullOrEmpty(value))
                        {
                            Clear();
                            return null;
                        }

                        var parts = value.Split('|');

                        if (parts.Length < 2)
                        {
                            Clear();
                            return null;
                        }

                        if (string.IsNullOrWhiteSpace(parts[1]))
                            return null;

                        return parts[1].FromJson<SalesPortalSession>();

                    }
                    catch (Exception ex)
                    {
                        // Unable to decrypt means that there was a cookie but whatever is in there is screwed.
                        // So let's clear it and return null so that it can get recreated again
                        _loggingService.Error($"Error getting the session cookie. {ex}");

                        Clear();
                    }
                }

                return null;
            }
        }

        public void Clear()
        {
            _cookieService.ClearCookie(CookieName);
        }

        public void ExtendSession()
        {
            if (HasValue())
            {
                Clear();
                Set(SalesPortalSession);
            }
        }

        public bool HasValue()
        {
            var sessionValue = SalesPortalSession;
            return sessionValue != null && !string.IsNullOrEmpty(sessionValue.UserName) && sessionValue.Roles.Any();
        }
    }
}
using System;
using System.Web.Helpers;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.Web.Shared.Cookie;

namespace TAL.QuoteAndApply.Web.Shared.Session
{
    public class SessionData
    {
        public bool CallBackRequested { get; set; }
    }

    public class QuoteSession
    {
        public SessionData SessionData { get; set; }
        public string QuoteReference { get; set; }
    }

    public interface IQuoteSessionContext
    {
        QuoteSession QuoteSession { get; }
        void Set(QuoteSession quoteSession);
        void Clear();
        void ExtendSession();
        bool HasValue();
        void Set(string quoteReference);
        void ExtendSessionWithChatCallBack(string quoteReference);
    }

    public class QuoteSessionContext : IQuoteSessionContext
    {
        private readonly ISecurityService _securityService;
        private readonly ICookieService _cookieService;

        public QuoteSessionContext(ISecurityService securityService, ICookieService cookieService)
        {
            _securityService = securityService;
            _cookieService = cookieService;
        }

        private const string CookieName = "MnJAMeVjXZFVdHmT"; //TODO: generate random cookie name (this CUE's one atm)

        public QuoteSession QuoteSession
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

                        var returnObj = new QuoteSession(){ QuoteReference = parts[1] };

                        if (parts.Length == 3 && !string.IsNullOrWhiteSpace(parts[2]))
                        {
                            returnObj.SessionData = Json.Decode<SessionData>(parts[2]);
                        }

                        return returnObj;
                    }
                    catch (Exception ex)
                    {
                        // Unable to decrypt means that there was a cookie but whatever is in there is screwed.
                        // So let's clear it and return null so that it can get recreated again
                        //TODO: log error

                        Clear();
                    }
                }

                return null;
            }
        }

        public void Set(QuoteSession quoteSession)
        {
            Random generator = new Random();
            var randomString = generator.Next(0, 1000000).ToString("D6");

            var quoteSessionObject = Json.Encode(quoteSession.SessionData);
            var stringToEncrypt = string.Format("{0}|{1}|{2}", randomString, quoteSession.QuoteReference, quoteSessionObject);

            //TODO: session timeout to config setting
            _cookieService.SetCookie(CookieName, _securityService.Encrypt(stringToEncrypt), DateTime.Now.AddMinutes(Convert.ToDouble(60)));
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
                Set(QuoteSession);
            }
        }

        public void ExtendSessionWithChatCallBack(string quoteReference)
        {
            if (HasValue())
            {
                Clear();
                Set(new QuoteSession()
                {
                    QuoteReference = quoteReference,
                    SessionData = new SessionData() { CallBackRequested = true }
                });
            }
        }

        public bool HasValue()
        {
            return !string.IsNullOrWhiteSpace(QuoteSession?.QuoteReference);
        }

        public void Set(string quoteReference)
        {
            Set(new QuoteSession()
            {
                QuoteReference = quoteReference,
                SessionData = new SessionData() { CallBackRequested = false }
            });
        }
    }
}

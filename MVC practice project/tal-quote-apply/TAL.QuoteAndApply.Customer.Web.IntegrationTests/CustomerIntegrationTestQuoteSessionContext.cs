using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests
{
    public class CustomerIntegrationTestQuoteSessionContext : IQuoteSessionContext
    {
        private static string _quoteReference;
        private QuoteSession _quoteSession = new QuoteSession()
        {
            QuoteReference = _quoteReference,
            SessionData = new SessionData()
            {
                CallBackRequested = false
            }
        };

        public QuoteSession QuoteSession
        {
            get { return _quoteSession; }
        }

        public void Set(QuoteSession quoteSession)
        {
            _quoteSession = quoteSession;
        }

        public void Clear()
        {
            _quoteSession = null;
        }

        public void ExtendSession()
        {
            //I don't always Extend Sessions, but when I do, I don't do anything
        }

        public bool HasValue()
        {
            return !string.IsNullOrWhiteSpace(_quoteSession?.QuoteReference);
        }

        public void Set(string quoteReference)
        {
            Set(new QuoteSession()
            {
                QuoteReference = quoteReference,
                SessionData = new SessionData() { CallBackRequested = _quoteSession?.SessionData?.CallBackRequested ?? false }
            });
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
    }
}

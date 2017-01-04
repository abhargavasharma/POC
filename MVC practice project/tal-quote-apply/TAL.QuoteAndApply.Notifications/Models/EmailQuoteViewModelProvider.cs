using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Notifications.Configuration;

namespace TAL.QuoteAndApply.Notifications.Models
{
    public interface IEmailQuoteViewModelProvider
    {
        EmailQuoteViewModel From(string quoteReferenceNumber, string emailAddress, string firstName, string userEmailAddress, string brandkey);
    }

    public class EmailQuoteViewModelProvider : IEmailQuoteViewModelProvider
    {
        private readonly IEmailConfigurationProvider _emailConfigurationProvider;
        private readonly ICurrentUrlProvider _currentUrlProvider;

        public EmailQuoteViewModelProvider(IEmailConfigurationProvider emailConfigurationProvider, ICurrentUrlProvider currentUrlProvider)
        {
            _emailConfigurationProvider = emailConfigurationProvider;
            _currentUrlProvider = currentUrlProvider;
        }

        public EmailQuoteViewModel From(string quoteReferenceNumber, string emailAddress, string firstName, string userEmailAddress, string brandKey)
        {
            var liveCustomerPortalBaseUrl = _currentUrlProvider.GetCurrentBaseUrl();
            return new EmailQuoteViewModel
            {
                UserEmailAddress = userEmailAddress ?? _emailConfigurationProvider.DefaultSenderEmailAddress,
                QuoteReferenceNumber = quoteReferenceNumber,
                EmailAddress = emailAddress,
                FirstName = firstName.ToTitleCase(),
                ImgPaths = new ImagePaths()
                {
                    Header_01 = _emailConfigurationProvider.Header_01,
                    Header_02 = _emailConfigurationProvider.Header_02,
                    Header_03 = _emailConfigurationProvider.Header_03,
                    Icon_australia = _emailConfigurationProvider.Icon_australia,
                    Icon_coins = _emailConfigurationProvider.Icon_coins,
                    Icon_people = _emailConfigurationProvider.Icon_people,
                    Full_width_spacer = _emailConfigurationProvider.Full_width_spacer,
                    Tal_bicycle_man = _emailConfigurationProvider.Tal_bicycle_man,
                    Logo = _emailConfigurationProvider.Logo,
                    Icon_Tick = _emailConfigurationProvider.Icon_tick,
                    Placeholder_tall_mum = _emailConfigurationProvider.Placeholder_tall_mum,
                },
                RetrieveUrl = string.Concat(liveCustomerPortalBaseUrl, "/retrieve?id=", quoteReferenceNumber),
                PdsUrl = _emailConfigurationProvider.LiveCustomerPortalPdsUrl,
                AllowEmail = IsInternalEmailAddress(emailAddress) 
                    || (_emailConfigurationProvider.AllowExternalEmails && !IsInternalEmailAddress(emailAddress))
            };
        }

        public bool IsInternalEmailAddress(string emailAddress)
        {
            return emailAddress.Contains("@tal.com.au");
        }
    }
}


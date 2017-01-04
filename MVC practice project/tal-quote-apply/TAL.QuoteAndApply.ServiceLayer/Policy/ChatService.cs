using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IChatService
    {
        string GetWebChatUrl(string quoteReferenceNumber);
        string WebChatStartTime { get; }
        string WebChatEndTime { get; }
        bool IsWebChatAvailable();
        bool IsWebChatAvailableAndCreateInteraction(string quoteReferenceNumber);
        Task<bool> RequestCallback(string quoteReferenceNumber, SaveCustomerParam saveRequest);
    }

    public class ChatService: IChatService
    {
        private readonly IChatConfigurationProvider _chatConfigurationProvider;
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly ICustomerSaveService _customerSaveService;
        private readonly IUrlUtilities _urlUtilities;
        private readonly ICustomerCallbackService _customerCallbackService;
        private readonly IPolicyInteractionService _policyInteractionService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ChatService(IPolicyOverviewProvider policyOverviewProvider, IDateTimeProvider dateTimeProvider, ICustomerSaveService customerSaveService, IUrlUtilities urlUtilities, ICustomerCallbackService customerCallbackService, IChatConfigurationProvider chatConfigurationProvider, IPolicyInteractionService policyInteractionService)
        {
            _policyOverviewProvider = policyOverviewProvider;
            _customerSaveService = customerSaveService;
            _urlUtilities = urlUtilities;
            _customerCallbackService = customerCallbackService;
            _chatConfigurationProvider = chatConfigurationProvider;
            _policyInteractionService = policyInteractionService;
            _dateTimeProvider = dateTimeProvider;
        }

        public string WebChatStartTime => _chatConfigurationProvider.StartTime;
        public string WebChatEndTime => _chatConfigurationProvider.EndTime;

        public string GetWebChatUrl(string quoteReferenceNumber)
        {   
            var chatUrl = $"{_chatConfigurationProvider.WebChatUrl}";

            if (!string.IsNullOrEmpty(quoteReferenceNumber))
            {
                var policyOverview = _policyOverviewProvider.GetFor(quoteReferenceNumber);

                //todo: assuming first risk
                var firstRisk = policyOverview?.Risks.FirstOrDefault();
                if (firstRisk != null)
                {
                    var party = _customerSaveService.GetPartyByRiskId(firstRisk.RiskId);

                    chatUrl += $"?firstName={_urlUtilities.UrlEncode(party.FirstName)}" +
                               $"&surname={_urlUtilities.UrlEncode(party.Surname)}" +
                               $"&quoteReferenceNo={_urlUtilities.UrlEncode(quoteReferenceNumber)}" +
                               $"&phoneNo={_urlUtilities.UrlEncode(GetPhoneNumber(party))}" +
                               $"&email={_urlUtilities.UrlEncode(party.EmailAddress)}";
                }
            }

            return chatUrl;
        }

        public bool IsWebChatAvailable()
        {
            var currentServerTime = _dateTimeProvider.GetCurrentDateAndTime();

            const string timePattern = @"^([01]\d|2[0-3]):([0-5]\d)$";

            if (string.IsNullOrEmpty(WebChatStartTime) || string.IsNullOrEmpty(WebChatEndTime))
            {
                throw new Exception("Unable to find start or end time in configuration");
            }

            if (!Regex.IsMatch(WebChatStartTime, timePattern) || !Regex.IsMatch(WebChatEndTime, timePattern))
            {
                throw new Exception("Webchat time not configured correctly.  Please check the configuration file.");
            }

            // split time into hours and minutes
            var start = WebChatStartTime.Split(':').Select(Int32.Parse).ToArray();
            var end = WebChatEndTime.Split(':').Select(Int32.Parse).ToArray();

            // generate start and end hour DateTime for comparison
            var startHour = new DateTime(currentServerTime.Year, currentServerTime.Month, currentServerTime.Day,
                start[0], start[1], 0, DateTimeKind.Local);

            var endHour = new DateTime(currentServerTime.Year, currentServerTime.Month, currentServerTime.Day,
                end[0], end[1], 0, DateTimeKind.Local);


            return currentServerTime >= startHour && currentServerTime < endHour;
        }

        public bool IsWebChatAvailableAndCreateInteraction(string quoteReferenceNumber)
        {
            var isWebChatAvailable = IsWebChatAvailable();
            if (isWebChatAvailable && !String.IsNullOrEmpty(quoteReferenceNumber))
            {
                _policyInteractionService.CustomerWebChatRequested(quoteReferenceNumber);
            }
            return isWebChatAvailable;
        }

        public async Task<bool> RequestCallback(string quoteReferenceNumber, SaveCustomerParam saveRequest)
        {
            var partyId = _customerSaveService.SaveCustomerWithoutUpdatingPolicyStatus(quoteReferenceNumber, saveRequest);
            var callbackResponse = await _customerCallbackService.RequestCallback(partyId, saveRequest.PhoneNumber);

            _policyInteractionService.CustomerCallbackRequested(quoteReferenceNumber);

            return callbackResponse;
        }

        //todo: refactor as extension
        private static string GetPhoneNumber(IParty party)
        {
            return string.IsNullOrEmpty(party.MobileNumber) ? party.HomeNumber : party.MobileNumber;
        }
    }
}

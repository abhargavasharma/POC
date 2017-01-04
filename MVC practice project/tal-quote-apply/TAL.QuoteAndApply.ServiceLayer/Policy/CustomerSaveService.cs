using System;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Notifications.Models;
using TAL.QuoteAndApply.Notifications.Service;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.RulesProxy;
using TAL.QuoteAndApply.UserRoles.Customer;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface ICustomerSaveService
    {
        int SaveCustomer(string quoteReferenceNumber, SaveCustomerParam saveCustomerParam);
        bool CreateLogin(string quoteReferenceNumber, CreateLoginParam createLoginParam);
        bool AccountExists(string emailAddress);
        int SaveCustomerWithoutUpdatingPolicyStatus(string quoteReferenceNumber, SaveCustomerParam saveCustomerParam);
        IParty GetPartyByRiskId(int riskId);
        SaveCustomerParam GetSaveCustomerParamByRiskId(int riskId, bool callBackSubmitted);
        bool SendEmail(int riskid, string quoteReferenceNumber, string brandKey);
    }

    public class CustomerSaveService : ICustomerSaveService
    {
        private readonly IPartyService _partyService;
        private readonly IRiskService _riskService;
		private readonly IPartyConsentService _partyConsentService;
        private readonly ICustomerAuthenticationService _customerAuthenticationService;
        private readonly IPolicyService _policyService;
        private readonly IPolicyInteractionService _interactionService;
        private readonly IEmailQuoteService _emailQuoteService;
        private readonly IPolicySourceProvider _policySourceProvider;
        private readonly IGenericRules _genericRules;

        public CustomerSaveService(IPartyService partyService,
            IRiskService riskService,
            ICustomerAuthenticationService customerAuthenticationService,
            IPolicyService policyService, 
            IPartyConsentService partyConsentService,
            IPolicyInteractionService interactionService, 
            IEmailQuoteService emailQuoteService, 
            IPolicySourceProvider policySourceProvider, 
            IGenericRules genericRules)
        {
            _partyService = partyService;
            _riskService = riskService;
            _customerAuthenticationService = customerAuthenticationService;
            _policyService = policyService;
            _partyConsentService = partyConsentService;
            _interactionService = interactionService;
            _emailQuoteService = emailQuoteService;
            _policySourceProvider = policySourceProvider;
            _genericRules = genericRules;
        }

        public int SaveCustomer(string quoteReferenceNumber, SaveCustomerParam saveCustomerParam)
        {
            var party = GetParty(saveCustomerParam);
            var policySource = _policySourceProvider.From(quoteReferenceNumber);

            //step one : Update Party and Party consent
            UpdateParty(saveCustomerParam, party, policySource);
            UpdatePartyConsent(saveCustomerParam, party);

            UpdatePolicyStatus(quoteReferenceNumber);

            return party.Id;
        }

        public int SaveCustomerWithoutUpdatingPolicyStatus(string quoteReferenceNumber, SaveCustomerParam saveCustomerParam)
        {
            var party = GetParty(saveCustomerParam);
            var policySource = _policySourceProvider.From(quoteReferenceNumber);

            //step one : Update Party and Party consent
            UpdateParty(saveCustomerParam, party, policySource);
            UpdatePartyConsent(saveCustomerParam, party);
            
            return party.Id;
        }

        public bool CreateLogin(string quoteReferenceNumber, CreateLoginParam createLoginParam)
        {
            var risk = _riskService.GetRisk(createLoginParam.RiskId);
            var party = _partyService.GetParty(risk.PartyId);
            var createLoginResult = new CreateCustomerLoginResult();
            if (party != null)
            {
                createLoginResult = _customerAuthenticationService.CreateCustomerLogin(
                    quoteReferenceNumber,
                    party.EmailAddress,
                    createLoginParam.Password,
                    party.FirstName,
                    party.Surname);
            }

            if (createLoginResult.Status != CustomerResultStatus.Success)
            {
                return false;
            }

            _policyService.UpdatePolicySaveStatus(quoteReferenceNumber, PolicySaveStatus.CreatedLogin);
            return true;
        }

        public bool AccountExists(string emailAddress)
        {
            return _customerAuthenticationService.AccountExists(emailAddress);
        }

        private void UpdateParty(SaveCustomerParam saveCustomerParam, IParty party, PolicySource policySource)
        {
            party.FirstName = saveCustomerParam.FirstName;
            party.Surname = saveCustomerParam.LastName;
            party.EmailAddress = saveCustomerParam.EmailAddress;

            if (_genericRules.IsValidMobilePrefixRule(saveCustomerParam.PhoneNumber))
            {
                party.MobileNumber = saveCustomerParam.PhoneNumber;
            }
            else
            {
                party.HomeNumber = saveCustomerParam.PhoneNumber;
            }
            
            _partyService.UpdateParty(party, policySource);
        }

        private void UpdatePartyConsent(SaveCustomerParam saveCustomerParam, IParty party)
        {
            var partyConsent = _partyConsentService.GetPartyConsentByPartyId(party.Id);
            partyConsent.ExpressConsent = saveCustomerParam.ExpressConsent;
            partyConsent.ExpressConsentUpdatedTs = DateTime.Now;
            _partyConsentService.UpdatePartyConsent(partyConsent, party);
        }

        private IParty GetParty(SaveCustomerParam saveCustomerParam)
        {
            if (!saveCustomerParam.RiskId.HasValue)
            {
                throw new Exception("Risk Id is requird");
            }

            return GetPartyByRiskId(saveCustomerParam.RiskId.Value);
        }

        public IParty GetPartyByRiskId(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            var party = _partyService.GetParty(risk.PartyId);
            return party;
        }

        public SaveCustomerParam GetSaveCustomerParamByRiskId(int riskId, bool callBackSubmitted)
        {
            var party = GetPartyByRiskId(riskId);
            return new SaveCustomerParam()
            {
                EmailAddress = party.EmailAddress,
                FirstName = party.FirstName,
                LastName = party.Surname,
                PhoneNumber = party.MobileNumber.IsNullOrWhiteSpace() ? party.HomeNumber : party.MobileNumber,
                RiskId = riskId,
                CallBackSubmitted = callBackSubmitted
            };
        }

        public bool SendEmail(int riskId, string quoteReferenceNumber, string brandKey)
        {
            var risk = _riskService.GetRisk(riskId);
            var party = _partyService.GetParty(risk.PartyId);
            if (party != null)
            {
                var emailSent = _emailQuoteService.SendQuote(
                    quoteReferenceNumber,
                    party.EmailAddress,
                    party.FirstName,
                    brandKey);
                _interactionService.QuoteSavedEmailSent(quoteReferenceNumber);
                return emailSent;
            }
            return false;
        }

        private void UpdatePolicyStatus(string quoteReferenceNumber)
        {
            _policyService.UpdatePolicySaveStatus(quoteReferenceNumber, PolicySaveStatus.PersonalDetailsEntered);

            //Note: save interaction is when they've entered personal details, not when they've created the login
            _interactionService.PolicySavedByCustomer(quoteReferenceNumber);
        }
        
    }
}

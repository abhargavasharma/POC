using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyOverviewProvider
    {
        PolicyOverviewResult GetFor(string quoteReferenceNumber);
        RiskOverviewResult GetFor(string quoteReferenceNumber, int riskId);
        PolicyOwnerDetailsParam GetOwnerDetailsFor(string quoteReferenceNumber);
    }

    public class PolicyOverviewProvider : IPolicyOverviewProvider
    {
        private readonly IPolicyService _policyService;
        private readonly IPartyService _partyService;
        private readonly IRiskOccupationService _riskOccupationService;
        private readonly IPolicyOverviewResultConverter _policyOverviewResultConverter;
        private readonly IRiskOverviewResultConverter _riskOverviewResultConverter;
        private readonly ICustomerPolicyRiskService _customerPolicyRiskService;
        private readonly IPolicyOwnerService _policyOwnerService;
        private readonly IPartyConsentService _partyConsentService;
        private readonly IPartyConsentParamConverter _partyConsentParamConverter;

        public PolicyOverviewProvider(IPolicyService policyService, IPartyService partyService,
            IPolicyOverviewResultConverter policyOverviewResultConverter,
            IRiskOverviewResultConverter riskOverviewResultConverter, 
            ICustomerPolicyRiskService customerPolicyRiskService, 
            IRiskOccupationService riskOccupationService, 
            IPolicyOwnerService policyOwnerService, 
            IPartyConsentService partyConsentService, 
            IPartyConsentParamConverter partyConsentParamConverter)
        {
            _policyService = policyService;
            _partyService = partyService;
            _policyOverviewResultConverter = policyOverviewResultConverter;
            _riskOverviewResultConverter = riskOverviewResultConverter;
            _customerPolicyRiskService = customerPolicyRiskService;
            _riskOccupationService = riskOccupationService;
            _policyOwnerService = policyOwnerService;
            _partyConsentService = partyConsentService;
            _partyConsentParamConverter = partyConsentParamConverter;
        }

        public PolicyOverviewResult GetFor(string quoteReferenceNumber)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            var risks = _policyService.GetRisksForPolicy(policy).ToList();
            var returnVal = _policyOverviewResultConverter.CreateFrom(policy, risks.FirstOrDefault()?.Id);
            returnVal.OwnerType = _policyOwnerService.GetPolicyOwnerType(policy.Id) ?? PolicyOwnerType.Ordinary;

            foreach (var r in risks)
            {
                returnVal.Risks.Add(GetRiskOverviewResult(r));
            }

            return returnVal;
        }

        public PolicyOwnerDetailsParam GetOwnerDetailsFor(string quoteReferenceNumber)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            var partyId = _policyOwnerService.GetPolicyOwnerPartyId(policy.Id).Value;

            var fundName = _policyOwnerService.GetPolicyOwnerFundName(policy.Id);

            var party = _partyService.GetParty(partyId);
            var partyConsent = _partyConsentService.GetPartyConsentByPartyId(partyId);
            var partyConsentResult = _partyConsentParamConverter.CreateFrom(partyConsent);

            var ownerDetails= new PolicyOwnerDetailsParam
            {
                FundName = fundName,

                Title = party.Title.ToString(),
                FirstName = party.FirstName,
                Surname = party.Surname,

                ExternalCustomerReference = party.ExternalCustomerReference,

                Address = party.Address,
                Suburb = party.Suburb,
                Postcode = party.Postcode,
                State = party.State.ToString(),

                MobileNumber = party.MobileNumber,
                HomeNumber = party.HomeNumber,
                EmailAddress = party.EmailAddress,

                PartyConsentsParam = partyConsentResult
            };

            
            return ownerDetails;
        }

        public RiskOverviewResult GetFor(string quoteReferenceNumber, int riskId)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            var risks = _policyService.GetRisksForPolicy(policy);
            var risk = risks.Single(r => r.Id == riskId);
            return GetRiskOverviewResult(risk);
        }

        private RiskOverviewResult GetRiskOverviewResult(IRisk risk)
        {
            var party = _partyService.GetParty(risk.PartyId);
            var availableDefinitions = _riskOccupationService.GetAvailableDefinitions(risk);
            var riskReturnVal = _riskOverviewResultConverter.CreateFrom(risk, availableDefinitions, party);

            return riskReturnVal;
        }
    }
}

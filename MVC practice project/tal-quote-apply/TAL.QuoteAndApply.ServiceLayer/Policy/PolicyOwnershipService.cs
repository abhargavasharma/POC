using System;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyOwnershipService
    {
        void SetSuperOwnership(string quoteReferenceNumber);
        void SetSmsfOwnership(string quoteReferenceNumber);
        void SetOrdinaryOwnership(string quoteReferenceNumber);
        void UpdateOwnerPartyDetails(string quoteReferenceNumber, PolicyOwnerDetailsParam ownerDetails);

        PolicyOwnerType GetPolicyOwnerType(string quoteReferenceNumber);
    }

    public class PolicyOwnershipService: IPolicyOwnershipService
    {
        private readonly IPartyService _partyService;
        private readonly IPolicyService _policyService;
        private readonly IPolicyOwnerService _policyOwnerService;
        private readonly IPartyConsentService _partyConsentService;
        private readonly IPartyConsentDtoUpdater _partyConsentDtoUpdater;
        private readonly IRiskService _riskService;

        public PolicyOwnershipService(IPartyService partyService, IPolicyService policyService, IPolicyOwnerService policyOwnerService, IPartyConsentService partyConsentService, IPartyConsentDtoUpdater partyConsentDtoUpdater, IRiskService riskService)
        {
            _partyService = partyService;
            _policyService = policyService;
            _policyOwnerService = policyOwnerService;
            _partyConsentService = partyConsentService;
            _partyConsentDtoUpdater = partyConsentDtoUpdater;
            _riskService = riskService;
        }

        public void SetSuperOwnership(string quoteReferenceNumber)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);

            var primaryRisk = _policyService.GetRisksForPolicy(policy).First();
            var ownerPartyId = _policyOwnerService.GetPolicyOwnerPartyId(policy.Id).Value;
            
            //for Super ownership the owner and the primary risk should be different
            if (primaryRisk.PartyId == ownerPartyId)
            {
                var ownerParty = _partyService.GetParty(ownerPartyId);
                CreateNewPartyForPrimaryRisk(primaryRisk, ownerParty);
            }
            
            _policyOwnerService.SetPolicyOwnerTypeForPolicy(policy.Id, PolicyOwnerType.SuperannuationFund);
            _policyOwnerService.SetPolicyOwnerFundName(policy.Id, "TASL / TAL Superfund");
        }

        public void SetSmsfOwnership(string quoteReferenceNumber)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);

            var primaryRisk = _policyService.GetRisksForPolicy(policy).First();
            var ownerPartyId = _policyOwnerService.GetPolicyOwnerPartyId(policy.Id).Value;

            //for SMSF ownership the owner and the primary risk should be different
            if (primaryRisk.PartyId == ownerPartyId)
            {
                var ownerParty = _partyService.GetParty(ownerPartyId);
                CreateNewPartyForPrimaryRisk(primaryRisk, ownerParty);
            }

            _policyOwnerService.SetPolicyOwnerTypeForPolicy(policy.Id, PolicyOwnerType.SelfManagedSuperFund);
            _policyOwnerService.SetPolicyOwnerFundName(policy.Id, "");
        }

        public void SetOrdinaryOwnership(string quoteReferenceNumber)
        {
            //load primary risk
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            var primaryRisk = _policyService.GetRisksForPolicy(policy).First();

            var ownerPartyId = _policyOwnerService.GetPolicyOwnerPartyId(policy.Id).Value;
            var primaryRiskPartyId = primaryRisk.PartyId;

            //for ordinary ownership both owner and primary risk should point to the same party
            if (ownerPartyId != primaryRiskPartyId)
            {
                //point primary risk to the owner risk
                primaryRisk.PartyId = ownerPartyId;
                _riskService.UpdateRisk(primaryRisk);

                //set ownerType to Ordinary
                _policyOwnerService.SetPolicyOwnerTypeForPolicy(policy.Id, PolicyOwnerType.Ordinary);
                _policyOwnerService.SetPolicyOwnerFundName(policy.Id, null);

                //delete orphaned party
                _partyService.DeleteParty(primaryRiskPartyId);
            }
        }

        public void UpdateOwnerPartyDetails(string quoteReferenceNumber, PolicyOwnerDetailsParam ownerDetails)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            //get party for the owner
            var ownerPartyId = _policyOwnerService.GetPolicyOwnerPartyId(policy.Id).Value;
            var ownerParty = _partyService.GetParty(ownerPartyId);

            //update party details
            ownerParty.FirstName = ownerDetails.FirstName;
            ownerParty.Surname = ownerDetails.Surname;
            ownerParty.Title = ownerDetails.Title.MapToTitle();

            ownerParty.ExternalCustomerReference = ownerDetails.ExternalCustomerReference;

            ownerParty.Address = ownerDetails.Address;
            ownerParty.Suburb = ownerDetails.Suburb;
            ownerParty.State = ownerDetails.State.MapToState();
            ownerParty.Postcode = ownerDetails.Postcode;

            ownerParty.EmailAddress = ownerDetails.EmailAddress;
            ownerParty.MobileNumber = ownerDetails.MobileNumber;
            ownerParty.HomeNumber = ownerDetails.HomeNumber;

            //update 
            _partyService.UpdateParty(ownerParty, policy.Source);
            _policyOwnerService.SetPolicyOwnerFundName(policy.Id, ownerDetails.FundName);

            //update party consent
            var partyConsent = _partyConsentService.GetPartyConsentByPartyId(ownerParty.Id);
            if (ownerDetails.PartyConsentsParam.ExpressConsent != partyConsent.ExpressConsent)
            {
                partyConsent.ExpressConsentUpdatedTs = DateTime.Now;
            }
            var updatedPartyConsent = _partyConsentDtoUpdater.UpdateFrom(partyConsent, ownerDetails.PartyConsentsParam);
            _partyConsentService.UpdatePartyConsent(updatedPartyConsent, ownerParty);
        }

        public PolicyOwnerType GetPolicyOwnerType(string quoteReferenceNumber)
        {
           return _policyOwnerService.GetOwnerShipType(quoteReferenceNumber);
        }

        private void CreateNewPartyForPrimaryRisk(IRisk primaryRisk, IParty ownerParty)
        {
            //create new party for risk with the minimum information, copied from the owner party
            var primaryRiskParty = _partyService.CreatePartyWithoutLead(new PartyDto { DateOfBirth = ownerParty.DateOfBirth, Gender = ownerParty.Gender });
            var partyConsentDto = new PartyConsentDto(primaryRiskParty.Id);
            _partyConsentService.CreatePartyConsent(partyConsentDto, primaryRiskParty);

            primaryRisk.PartyId = primaryRiskParty.Id;
            _riskService.UpdateRisk(primaryRisk);
        }
    }
}

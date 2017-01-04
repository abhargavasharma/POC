using System;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IPolicyOwnerService
    {
        void SetPolicyOwnerPartyForPolicy(int policyId, int partyId);
        int? GetPolicyOwnerPartyId(int policyId);
        void SetPolicyOwnerTypeForPolicy(int policyId, PolicyOwnerType ownerType);
        PolicyOwnerType? GetPolicyOwnerType(int policyId);
        void SetPolicyOwnerFundName(int policyId, string fundName);
        string GetPolicyOwnerFundName(int policyId);
        PolicyOwnerType GetOwnerShipType(string quoteReferenceNumber);

      

    }

    public class PolicyOwnerService : IPolicyOwnerService
    {
        private readonly IPolicyOwnerDtoRepository _policyOwnerDtoRepository;
        private readonly IPolicyService _policyService;
     
        public PolicyOwnerService(IPolicyOwnerDtoRepository policyOwnerDtoRepository,
            IPolicyService policyService)
        {
            _policyOwnerDtoRepository = policyOwnerDtoRepository;
            _policyService = policyService;
      }

        public int? GetPolicyOwnerPartyId(int policyId)
        {
            var currentAssoication = _policyOwnerDtoRepository.GetPolicyOwnerForPolicyId(policyId);
            return currentAssoication?.PartyId;
        }

        public void SetPolicyOwnerPartyForPolicy(int policyId, int partyId)
        {
            var currentAssoication = _policyOwnerDtoRepository.GetPolicyOwnerForPolicyId(policyId);

            if (currentAssoication == null)
            {
                _policyOwnerDtoRepository.Insert(new PolicyOwnerDto {PolicyId = policyId, PartyId = partyId});
            }
            else
            {
                currentAssoication.PartyId = partyId;
                _policyOwnerDtoRepository.Update(currentAssoication);
            }
        }

        public PolicyOwnerType? GetPolicyOwnerType(int policyId)
        {
            var currentOwner = _policyOwnerDtoRepository.GetPolicyOwnerForPolicyId(policyId);
            return currentOwner?.OwnerType;
        }

        public void SetPolicyOwnerTypeForPolicy(int policyId, PolicyOwnerType ownerType)
        {
            var currentAssoication = _policyOwnerDtoRepository.GetPolicyOwnerForPolicyId(policyId);

            currentAssoication.OwnerType = ownerType;
            _policyOwnerDtoRepository.Update(currentAssoication);
        }

        public void SetPolicyOwnerFundName(int policyId, string fundName)
        {
            var currentAssoication = _policyOwnerDtoRepository.GetPolicyOwnerForPolicyId(policyId);

            currentAssoication.FundName = fundName;
            _policyOwnerDtoRepository.Update(currentAssoication);
        }

        public string GetPolicyOwnerFundName(int policyId)
        {
            var currentAssoication = _policyOwnerDtoRepository.GetPolicyOwnerForPolicyId(policyId);

            return currentAssoication.FundName;
        }

        public PolicyOwnerType GetOwnerShipType(string quoteReferenceNumber)
        {
           
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNumber);
            var currentPolicyOwnerType = GetPolicyOwnerType(policy.Id)??PolicyOwnerType.Ordinary;
            return currentPolicyOwnerType;

        }

       
    }
}
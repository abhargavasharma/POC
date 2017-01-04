using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IPolicyService
    {
        IPolicy Get(int policyId);
        IPolicy GetByQuoteReferenceNumber(string quoteReferenceNumber);
        IEnumerable<IRisk> GetRisksForPolicy(IPolicy policy);
        IRisk GetRiskForPolicy(IPolicy policy, int riskId);
        IPolicy UpdateRaisedPolicyFields(IPolicy policy);
        IPolicy UpdatePolicyToReadyToInforce(IPolicy policy);
        IPolicy UpdatePolicyDeclarationAgree(IPolicy policy, bool declarationAgree);
        IPolicy UpdatePolicyPremiumFrequency(string quoteReferenceNumber, PremiumFrequency premiumFrequency);
        IPolicy UpdatePolicyToReferredToUnderwriter(IPolicy policy);
        IPolicy UpdatePolicyToIncomplete(IPolicy policy);
        void UpdatePolicy(IPolicy policy);
        void UpdatePolicySaveStatus(string quoteReferenceNumber, PolicySaveStatus saveStatus);
        void UpdatePolicyProgress(string quoteReferenceNumber, PolicyProgress progress);
        IPolicy UpdatePolicyToInforce(IPolicy policy);
        IPolicy UpdatePolicyToFailedToSendToPolicyAdminSystem(IPolicy policy);
        IPolicy UpdatePolicyToFailedDuringPolicyAdminSystemLoad(IPolicy policy);
    }
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyDtoRepository _policyDtoRepository;
        private readonly IBrandDtoRepository _brandDtoRepository;
        private readonly IRiskDtoRepository _riskDtoRepository;
        private readonly IRiskOccupationDtoRepository _riskOccupationDtoRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PolicyService(IPolicyDtoRepository policyDtoRepository, IRiskDtoRepository riskDtoRepository,
            IRiskOccupationDtoRepository riskOccupationDtoRepository, IDateTimeProvider dateTimeProvider, 
            IBrandDtoRepository brandDtoRepository)
        {
            _policyDtoRepository = policyDtoRepository;
            _riskDtoRepository = riskDtoRepository;
            _riskOccupationDtoRepository = riskOccupationDtoRepository;
            _dateTimeProvider = dateTimeProvider;
            _brandDtoRepository = brandDtoRepository;
        }

        public IPolicy GetByQuoteReferenceNumber(string quoteReferenceNumber)
        {
            var policy = _policyDtoRepository.GetPolicyByQuoteReference(quoteReferenceNumber);
            if (policy == null) return null;

            var brand = _brandDtoRepository.GetBrand(policy.BrandId);
            policy.BrandKey = brand.ProductKey;

            return policy;
        }

        public IPolicy UpdatePolicyToInforce(IPolicy policy)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(policy.QuoteReference);
            policyDto.Status = PolicyStatus.Inforce;
            UpdatePolicy(policyDto);
            return policy;
        }

        public IPolicy UpdatePolicyToFailedToSendToPolicyAdminSystem(IPolicy policy)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(policy.QuoteReference);
            policyDto.Status = PolicyStatus.FailedToSendToPolicyAdminSystem;
            UpdatePolicy(policyDto);
            return policy;
        }

        public IPolicy UpdatePolicyToFailedDuringPolicyAdminSystemLoad(IPolicy policy)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(policy.QuoteReference);
            policyDto.Status = PolicyStatus.FailedDuringPolicyAdminSystemLoad;
            UpdatePolicy(policyDto);
            return policy;
        }

        public IPolicy Get(int policyId)
        {
            var policy = _policyDtoRepository.GetPolicy(policyId);
            if (policy == null) return null;

            var brand = _brandDtoRepository.GetBrand(policy.BrandId);
            policy.BrandKey = brand.ProductKey;

            return policy;
        }

        public IEnumerable<IRisk> GetRisksForPolicy(IPolicy policy)
        {
            return _riskDtoRepository.GetRisksForPolicy(policy.Id).Select(risk =>
            {
                var occupation = _riskOccupationDtoRepository.GetForRisk(risk.Id);
                risk.AssignOccupationProperties(occupation);
                return risk;
            });
        }

        public IRisk GetRiskForPolicy(IPolicy policy, int riskId)
        {
            var risk = _riskDtoRepository.GetRisk(riskId);
            var occupation = _riskOccupationDtoRepository.GetForRisk(risk.Id);
            risk.AssignOccupationProperties(occupation);
            return risk;
        }

        public IPolicy UpdateRaisedPolicyFields(IPolicy policy)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(policy.QuoteReference);
            policyDto.Status = PolicyStatus.RaisedToPolicyAdminSystem;
            policyDto.SubmittedToRaiseTS = _dateTimeProvider.GetCurrentDateAndTime();
            UpdatePolicy(policyDto);
            return policy;
        }

        public IPolicy UpdatePolicyToReadyToInforce(IPolicy policy)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(policy.QuoteReference);
            policyDto.Status = PolicyStatus.ReadyForInforce;
            UpdatePolicy(policyDto);
            return policy;
        }

        public IPolicy UpdatePolicyDeclarationAgree(IPolicy policy, bool declarationAgree)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(policy.QuoteReference);
            policyDto.DeclarationAgree = declarationAgree;
            _policyDtoRepository.UpdatePolicy(policyDto);
            return policy;
        }

        public IPolicy UpdatePolicyPremiumFrequency(string quoteReferenceNumber, PremiumFrequency premiumFrequency)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(quoteReferenceNumber);
            policyDto.PremiumFrequency = premiumFrequency;
            UpdatePolicy(policyDto);

            var brand = _brandDtoRepository.GetBrand(policyDto.BrandId);
            policyDto.BrandKey = brand.ProductKey;

            return policyDto;
        }

        public IPolicy UpdatePolicyToReferredToUnderwriter(IPolicy policy)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(policy.QuoteReference);
            policyDto.Status = PolicyStatus.ReferredToUnderwriter;
            UpdatePolicy(policyDto);
            return policy;
        }

        public IPolicy UpdatePolicyToIncomplete(IPolicy policy)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(policy.QuoteReference);
            policyDto.Status = PolicyStatus.Incomplete;
            UpdatePolicy(policyDto);
            return policy;
        }

        public void UpdatePolicy(IPolicy policy)
        {
            _policyDtoRepository.UpdatePolicy((PolicyDto) policy);
        }

        public void UpdatePolicySaveStatus(string quoteReferenceNumber, PolicySaveStatus saveStatus)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(quoteReferenceNumber);
            policyDto.SaveStatus = saveStatus;
            UpdatePolicy(policyDto);
        }

        public void UpdatePolicyProgress(string quoteReferenceNumber, PolicyProgress progress)
        {
            var policyDto = _policyDtoRepository.GetPolicyByQuoteReference(quoteReferenceNumber);
            policyDto.Progress = progress;
            UpdatePolicy(policyDto);
        }
    }
}

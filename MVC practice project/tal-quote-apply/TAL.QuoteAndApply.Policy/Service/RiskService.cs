using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IRiskService
    {
        IRisk CreateRisk(IRisk risk);
        IRisk GetRisk(int riskId);
        IRisk GetRiskByInterviewId(string interviewId);
        IRisk GetRiskByPartyId(int partyId);
        void UpdateRisk(IRisk risk);
        void SetLatestConcurrencyToken(IRisk risk, string concurrencyToken);
        IBeneficiary AddOrUpdateBeneficiary(IRisk risk, IBeneficiary beneficiary);
        void RemoveBeneficiary(IRisk risk, IBeneficiary beneficiary);
        ICollection<IBeneficiary> GetBeneficiariesForRisk(IRisk risk);
		bool IsRiskValidForInforce(RiskProductDefinition riskProductDefinition, IRisk risk);
        IReadOnlyList<IBeneficiaryRelationshipDto> GetBeneficiaryRelationships();
    }

    public class RiskService : IRiskService
    {
        private readonly IRiskDtoRepository _riskDtoRepository;
        private readonly IRiskOccupationService _riskOccupationService;
		private readonly IRiskRulesService _riskRulesService;
		private readonly IBeneficiaryDtoRepository _beneficiaryDtoRepository;
        private readonly IBeneficiaryRelationshipDtoRepository _relationshipToTheInsuredDtoRepository;

        public RiskService(IRiskDtoRepository riskDtoRepository, IRiskRulesService riskRulesService, IBeneficiaryDtoRepository beneficiaryDtoRepository, IBeneficiaryRelationshipDtoRepository relationshipToTheInsuredDtoRepository, IRiskOccupationService riskOccupationService)
        {
            _riskDtoRepository = riskDtoRepository;
            _riskRulesService = riskRulesService;
            _beneficiaryDtoRepository = beneficiaryDtoRepository;
            _relationshipToTheInsuredDtoRepository = relationshipToTheInsuredDtoRepository;
            _riskOccupationService = riskOccupationService;
        }

        public IRisk CreateRisk(IRisk risk)
        {
            var riskResponse = _riskDtoRepository.InsertRisk((RiskDto) risk);
            riskResponse.AssignOccupationProperties(risk);
            _riskOccupationService.CreateForRisk(riskResponse);
            return riskResponse;
        }

        public IRisk GetRisk(int riskId)
        {
            var risk = _riskDtoRepository.GetRisk(riskId);
            return UpdateRiskWithOccupation(risk);
        }

        public IRisk GetRiskByInterviewId(string interviewId)
        {
            var risk = _riskDtoRepository.GetRiskByInterviewId(interviewId);
            return UpdateRiskWithOccupation(risk);
        }

        public IRisk GetRiskByPartyId(int partyId)
        {
            var risk = _riskDtoRepository.GetRiskByPartyId(partyId);
            return UpdateRiskWithOccupation(risk);
        }

        public void UpdateRisk(IRisk risk)
        {
            _riskDtoRepository.UpdateRisk((RiskDto)risk);
            _riskOccupationService.UpdateForRisk(risk);
        }

        public void SetLatestConcurrencyToken(IRisk risk, string concurrencyToken)
        {
            _riskDtoRepository.SetLatestConcurrencyToken((RiskDto) risk, concurrencyToken);
        }

        public bool IsRiskValidForInforce(RiskProductDefinition riskProductDefinition, IRisk risk)
        {
            return _riskRulesService.ValidateRiskForInforce(riskProductDefinition,risk).All(x => x.IsSatisfied);
        }
		
		public ICollection<IBeneficiary> GetBeneficiariesForRisk(IRisk risk)
        {
            return _beneficiaryDtoRepository.GetBeneficiariesForRisk(risk.Id).ToArray();
        }

        public IBeneficiary AddOrUpdateBeneficiary(IRisk risk, IBeneficiary beneficiary)
        {
            beneficiary.RiskId = risk.Id;
            if (beneficiary.Id > 0)
            {
                _beneficiaryDtoRepository.UpdateBeneficiary((BeneficiaryDto) beneficiary);
            }
            else
            {
                _beneficiaryDtoRepository.InsertBeneficiary((BeneficiaryDto)beneficiary);
            }

            return beneficiary;
        }

        public void RemoveBeneficiary(IRisk risk, IBeneficiary beneficiary)
        {
            _beneficiaryDtoRepository.DeleteBeneficiary((BeneficiaryDto)beneficiary);
        }

        public IReadOnlyList<IBeneficiaryRelationshipDto> GetBeneficiaryRelationships()
        {
            var relationships = _relationshipToTheInsuredDtoRepository.GetBeneficiaryRelationships();
            return relationships.ToList().AsReadOnly();
        }

        private IRisk UpdateRiskWithOccupation(IRisk risk)
        {
            if (risk != null)
            {
                var riskOccupation = _riskOccupationService.GetForRisk(risk);
                risk.AssignOccupationProperties(riskOccupation);
            }
            return risk;
        }
    }

    
}

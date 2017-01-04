using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary
{
    public interface IBeneficiaryDetailsService
    {
        RiskBeneficiaryDetailsParam CreateOrUpdateBeneficiary(RiskBeneficiaryDetailsParam beneficiaryDetailsParam, int riskId);
        ICollection<RiskBeneficiaryDetailsParam> GetBeneficiariesForRisk(int riskId);
        RiskBeneficiaryDetailsParam GetBeneficiaryForRisk(int riskId, int beneficiaryId);
        void UpdateLprForRisk(int riskId, bool lpr);
        bool GetLprForRisk(int riskId);
        void RemoveBeneficiary(int riskId, int beneficiaryId);
        IReadOnlyList<BeneficiaryRelationship> GetBeneficiaryRelationships();
    }

    public class BeneficiaryDetailsService : IBeneficiaryDetailsService
    {
        private readonly IBeneficiaryDtoConverter _beneficiaryDtoConverter;
        private readonly IBeneficiaryRelationshipConverter _relationshipToTheInsuredConverter;
        private readonly IRiskService _riskService;

        public BeneficiaryDetailsService(IBeneficiaryDtoConverter beneficiaryDtoConverter, IRiskService riskService, IBeneficiaryRelationshipConverter relationshipToTheInsuredConverter)
        {
            _beneficiaryDtoConverter = beneficiaryDtoConverter;
            _relationshipToTheInsuredConverter = relationshipToTheInsuredConverter;
            _riskService = riskService;
        }

        public void UpdateLprForRisk(int riskId, bool lpr)
        {
            var risk = _riskService.GetRisk(riskId);
            risk.LprBeneficiary = lpr;
            if (risk.LprBeneficiary)
            {
                var beneficaries = _riskService.GetBeneficiariesForRisk(risk);
                beneficaries.Do(beneficiary => _riskService.RemoveBeneficiary(risk, beneficiary));
            }
            _riskService.UpdateRisk(risk);
        }

        public bool GetLprForRisk(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            return risk.LprBeneficiary;
        }

        public void RemoveBeneficiary(int riskId, int beneficiaryId)
        {
            var risk = _riskService.GetRisk(riskId);
            var beneficiary = _riskService.GetBeneficiariesForRisk(risk).Single(b => b.Id == beneficiaryId);
            _riskService.RemoveBeneficiary(risk, beneficiary);
        }

        public RiskBeneficiaryDetailsParam CreateOrUpdateBeneficiary(RiskBeneficiaryDetailsParam beneficiaryDetailsParam, int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            IBeneficiary beneficiary;
            if (beneficiaryDetailsParam.Id > 0)
            {
                beneficiary = _riskService.GetBeneficiariesForRisk(risk).Single(b => b.Id == beneficiaryDetailsParam.Id);
                beneficiary = _beneficiaryDtoConverter.UpdateFrom(beneficiary, beneficiaryDetailsParam);
            }
            else
            {
                beneficiary = _beneficiaryDtoConverter.CreateFrom(beneficiaryDetailsParam);
            }
            var beneficiaryDto = _riskService.AddOrUpdateBeneficiary(risk, beneficiary);
            return _beneficiaryDtoConverter.CreateFrom(beneficiaryDto);
        }

        public ICollection<RiskBeneficiaryDetailsParam> GetBeneficiariesForRisk(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            var beneficiaries = _riskService.GetBeneficiariesForRisk(risk);
            return beneficiaries.Select(_beneficiaryDtoConverter.CreateFrom).ToArray();
        }

        public RiskBeneficiaryDetailsParam GetBeneficiaryForRisk(int riskId, int beneficiaryId)
        {
            var risk = _riskService.GetRisk(riskId);
            var beneficiaries = _riskService.GetBeneficiariesForRisk(risk);
            var beneficiary = beneficiaries.FirstOrDefault(b => b.Id == beneficiaryId);
            if (beneficiary == null)
            {
                return null;
            }

            return _beneficiaryDtoConverter.CreateFrom(beneficiary);
        }

        /// <summary>
        /// Get relationships to the insured 
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<BeneficiaryRelationship> GetBeneficiaryRelationships()
        {
            var relationships = _riskService.GetBeneficiaryRelationships();

            var relationshipToInsuredList = _relationshipToTheInsuredConverter.From(relationships);

            return relationshipToInsuredList;
        }
    }
}

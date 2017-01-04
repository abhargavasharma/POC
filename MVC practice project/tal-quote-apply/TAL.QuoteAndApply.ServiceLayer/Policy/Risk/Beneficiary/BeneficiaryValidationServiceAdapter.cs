using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary
{
    public interface IBeneficiaryValidationServiceAdapter
    {
        ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForInforceForRisk(int riskId);

        ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForInForce(IEnumerable<RiskBeneficiaryDetailsParam> beneficiaries, int riskId);

        ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForSave(
            IEnumerable<RiskBeneficiaryDetailsParam> beneficiaries);
    }

    public class BeneficiaryValidationServiceAdapter : IBeneficiaryValidationServiceAdapter
    {
        private readonly IBeneficiaryValidationService _validationService;
        private readonly IBeneficiaryDtoConverter _beneficiaryDtoConverter;

        public BeneficiaryValidationServiceAdapter(IBeneficiaryValidationService validationService, IBeneficiaryDtoConverter beneficiaryDtoConverter)
        {
            _validationService = validationService;
            _beneficiaryDtoConverter = beneficiaryDtoConverter;
        }

        public ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForInforceForRisk(int riskId)
        {
            return _validationService.ValidateBeneficiariesForInforceForRisk(riskId);
        }
        public ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForInForce(IEnumerable<RiskBeneficiaryDetailsParam> beneficiaries, int riskId)
        {
            return _validationService.ValidateBeneficiariesForInForce(beneficiaries.Select(_beneficiaryDtoConverter.CreateFrom), riskId);
        }

        public ICollection<RiskBeneficiaryValidationModel> ValidateBeneficiariesForSave(IEnumerable<RiskBeneficiaryDetailsParam> beneficiaries)
        {
            return _validationService.ValidateBeneficiariesForSave(beneficiaries.Select(_beneficiaryDtoConverter.CreateFrom));
        }
    }
}
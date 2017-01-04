using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IRiskOccupationService
    {
        RiskOccupationDto CreateForRisk(IRisk risk);
        RiskOccupationDto GetForRisk(IRisk risk);
        RiskOccupationDto GetForRiskId(int riskId);
        RiskOccupationDto UpdateForRisk(IRisk risk);
        List<OccupationDefinition> GetAvailableDefinitions(IRisk risk);
    }

    public class RiskOccupationService : IRiskOccupationService
    {
        private readonly IRiskOccupationDtoRepository _riskOccupationDtoRepository;

        public RiskOccupationService(IRiskOccupationDtoRepository riskOccupationDtoRepository)
        {
            _riskOccupationDtoRepository = riskOccupationDtoRepository;
        }

        public RiskOccupationDto CreateForRisk(IRisk risk)
        {
            var riskOccupation = new RiskOccupationDto
            {
                RiskId = risk.Id,
                IndustryCode = risk.IndustryCode,
                IndustryTitle = risk.IndustryTitle,
                IsTpdAny = risk.IsTpdAny,
                IsTpdOwn = risk.IsTpdOwn,
                OccupationCode = risk.OccupationCode,
                OccupationClass = risk.OccupationClass,
                OccupationTitle = risk.OccupationTitle,
                TpdLoading = risk.TpdLoading,
                PasCode = risk.PasCode
            };
            return _riskOccupationDtoRepository.Insert(riskOccupation);
        }

        public RiskOccupationDto GetForRisk(IRisk risk)
        {
            return GetForRiskId(risk.Id);
        }

        public RiskOccupationDto GetForRiskId(int riskId)
        {
            return _riskOccupationDtoRepository.GetForRisk(riskId);
        }

        public RiskOccupationDto UpdateForRisk(IRisk risk)
        {
            var occupationDto = _riskOccupationDtoRepository.GetForRisk(risk.Id);
            occupationDto.IndustryCode = risk.IndustryCode;
            occupationDto.IndustryTitle = risk.IndustryTitle;
            occupationDto.IsTpdAny = risk.IsTpdAny;
            occupationDto.IsTpdOwn = risk.IsTpdOwn;
            occupationDto.OccupationCode = risk.OccupationCode;
            occupationDto.OccupationClass = risk.OccupationClass;
            occupationDto.OccupationTitle = risk.OccupationTitle;
            occupationDto.TpdLoading = risk.TpdLoading;
            occupationDto.PasCode = risk.PasCode;
            _riskOccupationDtoRepository.UpdateOccupationRisk(occupationDto);
            
            return occupationDto;
        }

        public List<OccupationDefinition> GetAvailableDefinitions(IRisk risk)
        {
            var list = new List<OccupationDefinition>();

            var occupation = GetForRisk(risk);

            if (occupation.IsTpdAny)
            {
                list.Add(OccupationDefinition.AnyOccupation);
            }

            if (occupation.IsTpdOwn)
            {
                list.Add(OccupationDefinition.OwnOccupation);
            }

            return list;
        }
    }
}
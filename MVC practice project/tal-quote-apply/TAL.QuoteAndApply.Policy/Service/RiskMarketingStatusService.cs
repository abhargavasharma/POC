using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IRiskMarketingStatusService
    {
        IRiskMarketingStatus GetById(int id);
        IRiskMarketingStatus CreateRiskMarketingStatus(IRiskMarketingStatus riskMarketingStatusDto);
        IRiskMarketingStatus GetRiskMarketingStatusByRiskId(int riskId);
        void UpdateRiskMarketingStatus(IRiskMarketingStatus riskMarketingStatusDto);
    }

    public class RiskMarketingStatusService : IRiskMarketingStatusService
    {
        private readonly IRiskMarketingStatusDtoRepository _riskMarketingStatusDtoRepository;

        public RiskMarketingStatusService(IRiskMarketingStatusDtoRepository riskMarketingStatusDtoRepository)
        {
            _riskMarketingStatusDtoRepository = riskMarketingStatusDtoRepository;
        }

        public IRiskMarketingStatus GetById(int id)
        {
            return _riskMarketingStatusDtoRepository.GetRiskMarketingStatus(id);
        }

        public IRiskMarketingStatus CreateRiskMarketingStatus(IRiskMarketingStatus riskMarketingStatusDto)
        {
            return _riskMarketingStatusDtoRepository.InsertRiskMarketingStatus((RiskMarketingStatusDto)riskMarketingStatusDto);
        }

        public IRiskMarketingStatus GetRiskMarketingStatusByRiskId(int riskId)
        {
            return _riskMarketingStatusDtoRepository.GetRiskMarketingStatusByRiskId(riskId);
        }

        public void UpdateRiskMarketingStatus(IRiskMarketingStatus riskMarketingStatusDto)
        {
            _riskMarketingStatusDtoRepository.UpdateRiskMarketingStatus((RiskMarketingStatusDto)riskMarketingStatusDto);
        }
    }
}

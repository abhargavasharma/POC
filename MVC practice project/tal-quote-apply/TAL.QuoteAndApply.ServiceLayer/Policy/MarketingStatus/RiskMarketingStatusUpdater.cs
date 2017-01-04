using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.MarketingStatus;
using Status = TAL.QuoteAndApply.DataModel.Policy.MarketingStatus;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus
{
    public interface IRiskMarketingStatusUpdater
    {
        void CreateNewRiskMarketingStatus(IRisk risk, Status marketingStatus);
        Status UpdateRiskMarketingStatus(IRisk risk, Status[] plansMarketingStatusList);
    }

    public class RiskMarketingStatusUpdater : IRiskMarketingStatusUpdater
    {
        private readonly IRiskMarketingStatusService _riskMarketingStatusService;
        private readonly IPolicyService _policyService;
        private readonly IRiskMarketingStatusProvider _riskMarketingStatusProvider;

        public RiskMarketingStatusUpdater(IRiskMarketingStatusService riskMarketingStatusService,
            IPolicyService policyService,
            IRiskMarketingStatusProvider riskMarketingStatusProvider)
        {
            _riskMarketingStatusService = riskMarketingStatusService;
            _policyService = policyService;
            _riskMarketingStatusProvider = riskMarketingStatusProvider;
        }

        public void CreateNewRiskMarketingStatus(IRisk risk, Status marketingStatus)
        {
            var riskMarketingStatusDto = new RiskMarketingStatusDto()
            {
                RiskId = risk.Id,
                MarketingStatusId = marketingStatus
            };
            _riskMarketingStatusService.CreateRiskMarketingStatus(riskMarketingStatusDto);
        }

        public Status UpdateRiskMarketingStatus(IRisk risk, Status[] plansMarketingStatusList)
        {
            var marketingStatus = _riskMarketingStatusProvider.GetRiskMarketingStatus(plansMarketingStatusList);

            if (_policyService.Get(risk.PolicyId).Source == PolicySource.SalesPortal)
            {
                marketingStatus = Status.Unknown;
            }
            var riskmarketingStatus = _riskMarketingStatusService.GetRiskMarketingStatusByRiskId(risk.Id);
            if (riskmarketingStatus == null)
            {
                CreateNewRiskMarketingStatus(risk, Status.Unknown);
                riskmarketingStatus = _riskMarketingStatusService.GetRiskMarketingStatusByRiskId(risk.Id);
            }

            riskmarketingStatus.MarketingStatusId = marketingStatus;
            _riskMarketingStatusService.UpdateRiskMarketingStatus(riskmarketingStatus);
            return marketingStatus;
        }
    }
}

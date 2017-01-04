using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.MarketingStatus;
using Status = TAL.QuoteAndApply.DataModel.Policy.MarketingStatus;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus
{
    public interface ICoverMarketingStatusUpdater
    {
        void CreateNewCoverMarketingStatus(ICover risk);
        Status UpdateCoverMarketingStatus(ICover cover, bool coverEligibile);
    }

    public class CoverMarketingStatusUpdater : ICoverMarketingStatusUpdater
    {
        private readonly ICoverMarketingStatusService _coverMarketingStatusService;
        private readonly ICoverMarketingStatusProvider _coverMarketingStatusProvider;

        public CoverMarketingStatusUpdater(ICoverMarketingStatusService coverMarketingStatusService,
            ICoverMarketingStatusProvider coverMarketingStatusProvider)
        {
            _coverMarketingStatusService = coverMarketingStatusService;
            _coverMarketingStatusProvider = coverMarketingStatusProvider;
        }

        public void CreateNewCoverMarketingStatus(ICover risk)
        {
            var coverMarketingStatusDto = new CoverMarketingStatusDto()
            {
                CoverId = risk.Id
            };
            _coverMarketingStatusService.CreateCoverMarketingStatus(coverMarketingStatusDto);
        }

        public Status UpdateCoverMarketingStatus(ICover cover, bool eligibile)
        {
            var marketingStatus = _coverMarketingStatusProvider.GetCoverMarketingStatus(cover.Selected, eligibile, cover.UnderwritingStatus);
            var currentCoverMarketingStatus = _coverMarketingStatusService.GetCoverMarketingStatusByCoverId(cover.Id);
            if (currentCoverMarketingStatus == null)
            {
                CreateNewCoverMarketingStatus(cover);
                currentCoverMarketingStatus = _coverMarketingStatusService.GetCoverMarketingStatusByCoverId(cover.Id);
            }
            currentCoverMarketingStatus.MarketingStatusId = marketingStatus;
            _coverMarketingStatusService.UpdateCoverMarketingStatus(currentCoverMarketingStatus);
            return marketingStatus;
        }
    }
}

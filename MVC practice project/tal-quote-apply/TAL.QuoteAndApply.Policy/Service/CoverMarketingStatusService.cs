using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface ICoverMarketingStatusService
    {
        ICoverMarketingStatus GetById(int id);
        ICoverMarketingStatus CreateCoverMarketingStatus(ICoverMarketingStatus coverMarketingStatusDto);
        ICoverMarketingStatus GetCoverMarketingStatusByCoverId(int coverId);
        void UpdateCoverMarketingStatus(ICoverMarketingStatus coverMarketingStatusDto);
    }

    public class CoverMarketingStatusService : ICoverMarketingStatusService
    {
        private readonly ICoverMarketingStatusDtoRepository _coverMarketingStatusDtoRepository;

        public CoverMarketingStatusService(ICoverMarketingStatusDtoRepository coverMarketingStatusDtoRepository)
        {
            _coverMarketingStatusDtoRepository = coverMarketingStatusDtoRepository;
        }

        public ICoverMarketingStatus GetById(int id)
        {
            return _coverMarketingStatusDtoRepository.GetCoverMarketingStatus(id);
        }

        public ICoverMarketingStatus CreateCoverMarketingStatus(ICoverMarketingStatus coverMarketingStatusDto)
        {
            return _coverMarketingStatusDtoRepository.InsertCoverMarketingStatus((CoverMarketingStatusDto)coverMarketingStatusDto);
        }

        public ICoverMarketingStatus GetCoverMarketingStatusByCoverId(int coverId)
        {
            return _coverMarketingStatusDtoRepository.GetCoverMarketingStatusByCoverId(coverId);
        }

        public void UpdateCoverMarketingStatus(ICoverMarketingStatus coverMarketingStatusDto)
        {
            _coverMarketingStatusDtoRepository.UpdateCoverMarketingStatus((CoverMarketingStatusDto)coverMarketingStatusDto);
        }
    }
}

using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface ICoverMarketingStatusDtoRepository
    {
        CoverMarketingStatusDto InsertCoverMarketingStatus(CoverMarketingStatusDto coverMarketingStatus);
        CoverMarketingStatusDto GetCoverMarketingStatus(int id);
        CoverMarketingStatusDto GetCoverMarketingStatusByCoverId(int coverId);
        void UpdateCoverMarketingStatus(CoverMarketingStatusDto coverMarketingStatus);
    }

    public class CoverMarketingStatusDtoRepository : BaseRepository<CoverMarketingStatusDto>, ICoverMarketingStatusDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public CoverMarketingStatusDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public CoverMarketingStatusDto InsertCoverMarketingStatus(CoverMarketingStatusDto coverMarketingStatus)
        {
            var newDto = Insert(coverMarketingStatus);
            return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"CoverMarketingStatusId-{coverMarketingStatus.Id}", newDto);
        }

        public CoverMarketingStatusDto GetCoverMarketingStatus(int id)
        {
            return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"CoverMarketingStatusId-{id}", () => Get(id));
        }

        public CoverMarketingStatusDto GetCoverMarketingStatusByCoverId(int coverId)
        {
            var marketingStatuses = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"CoverId-{coverId}", 
                () => Where(coverMarketingStatus => coverMarketingStatus.CoverId, Op.Eq, coverId));
            return marketingStatuses.Select(m => GetCoverMarketingStatus(m.Id)).FirstOrDefault();
        }

        public void UpdateCoverMarketingStatus(CoverMarketingStatusDto coverMarketingStatus)
        {
            Update(coverMarketingStatus);
            var updatedDto = Get(coverMarketingStatus.Id);
            coverMarketingStatus.RV = updatedDto.RV;
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"CoverMarketingStatusId-{coverMarketingStatus.Id}", coverMarketingStatus);
        }
    }
}

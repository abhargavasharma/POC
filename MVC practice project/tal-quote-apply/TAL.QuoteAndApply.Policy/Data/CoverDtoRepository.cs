using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface ICoverDtoRepository
    {
        CoverDto InsertCover(CoverDto cover);
        CoverDto GetCover(int id);
        void UpdateCover(CoverDto cover);
        IEnumerable<CoverDto> GetCoversForPlan(int planId);
    }

    public class CoverDtoRepository : BaseRepository<CoverDto>, ICoverDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;
        private readonly ICoverChangeSubject _coverChangeSubject;

        public CoverDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper, ICoverChangeSubject coverChangeSubject)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
            _coverChangeSubject = coverChangeSubject;
        }

        public CoverDto InsertCover(CoverDto cover)
        {
            var newDto = Insert(cover);
            return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"CoverId-{cover.Id}", newDto);
        }

        public CoverDto GetCover(int id)
        {
            return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"CoverId-{id}", () => Get(id));
        }

        public void UpdateCover(CoverDto cover)
        {
            Update(cover);
            var updatedDto = Get(cover.Id);
            cover.RV = updatedDto.RV;
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"CoverId-{cover.Id}", cover);

            _coverChangeSubject.Notify(new ChangeEnvelope(cover));
        }

        public IEnumerable<CoverDto> GetCoversForPlan(int planId)
        {
            var covers = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"PlanId-{planId}", () => Where(cover => cover.PlanId, Op.Eq, planId));
            return covers.Select(c => GetCover(c.Id));
        }

        public new bool Delete(CoverDto cover)
        {
            var result = base.Delete(cover);

            if (result)
            {
                _cachingWrapper.RemoveItem(GetType(), $"CoverId-{cover.Id}");
            }

            return result;
        }
    }

}

using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IOptionDtoRepository
    {
        OptionDto InsertOption(OptionDto option);
        OptionDto GetOption(int id);
        void UpdateOption(OptionDto option);
        IEnumerable<OptionDto> GetOptionsForPlan(int planId);
    }

    public class OptionDtoRepository : BaseRepository<OptionDto>, IOptionDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public OptionDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public OptionDto InsertOption(OptionDto option)
        {
            var newDto = Insert(option);
            return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"OptionId-{option.Id}", newDto);
        }

        public OptionDto GetOption(int id)
        {
            return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"OptionId-{id}", () => Get(id));
        }

        public void UpdateOption(OptionDto option)
        {
            Update(option);
            var updatedDto = Get(option.Id);
            option.RV = updatedDto.RV;
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"OptionId-{option.Id}", option);
        }

        public IEnumerable<OptionDto> GetOptionsForPlan(int planId)
        {
            var options = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"PlanId-{planId}",
                () => Where(option => option.PlanId, Op.Eq, planId));
            
            return options.Select(o => GetOption(o.Id));
        }

        public new bool Delete(OptionDto option)
        {
            var result = base.Delete(option);

            if (result)
            {
                _cachingWrapper.RemoveItem(GetType(), $"OptionId-{option.Id}");
            }

            return result;
        }
    }

}

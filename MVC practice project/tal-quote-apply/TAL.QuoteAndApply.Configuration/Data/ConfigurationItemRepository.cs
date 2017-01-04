using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Configuration.Models;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;

namespace TAL.QuoteAndApply.Configuration.Data
{
    public interface IConfigurationItemRepository
    {
        ConfigurationItem GetConfigurationItem(int brandId, string configKey);
        void Update(ConfigurationItem item);
    }

    public class ConfigurationItemRepository : BaseRepository<ConfigurationItem>, IConfigurationItemRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public ConfigurationItemRepository(IApplicationConfigurationSettings settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(settings.DatabaseSettingsConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public ConfigurationItem GetConfigurationItem(int brandId, string configKey)
        {
            var query = DbItemPredicate<ConfigurationItem>.Where(p => p.BrandId, Op.Eq, brandId)
                .And(p => p.Key, Op.Eq, configKey);

            return _cachingWrapper.GetOrAddCacheItemSliding(GetType(), $"{brandId}-{configKey}", TimeSpan.FromMinutes(5), () => Query(query).FirstOrDefault());

        }
    }
}

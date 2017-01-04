using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IBrandDtoRepository
    {
        BrandDto GetBrand(int id);
        BrandDto GetBrand(string brandKey);
    }

    public class BrandDtoRepository: BaseRepository<BrandDto>, IBrandDtoRepository
    {
        private readonly ICachingWrapper _cache;

        public BrandDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cache)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cache = cache;
        }

        public BrandDto GetBrand(int id)
        {
            return Get(id);
        }

        public BrandDto GetBrand(string brandKey)
        {
            return
                _cache.GetOrAddCacheItemIndefinite(GetType(), $"BrandKey-{brandKey}",
                    () => Where(b => b.ProductKey, Op.Eq, brandKey)).SingleOrDefault();
        }
    }
}

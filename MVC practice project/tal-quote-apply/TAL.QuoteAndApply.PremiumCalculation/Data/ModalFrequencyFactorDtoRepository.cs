using System;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Data
{
    public interface IModalFrequencyFactorDtoRepository
    {
        ModalFrequencyFactorDto GetModalFrequencyFactorForPremiumFrequency(PremiumFrequency premiumFrequency, int brandId);
    }

    public class ModalFrequencyFactorDtoRepository : BaseRepository<ModalFrequencyFactorDto>, IModalFrequencyFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public ModalFrequencyFactorDtoRepository(IPremiumCalculationConfigurationProvider premiumCalculationConfigurationProvider, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(premiumCalculationConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public ModalFrequencyFactorDto GetModalFrequencyFactorForPremiumFrequency(PremiumFrequency premiumFrequency, int brandId)
        {
            var q = DbItemPredicate<ModalFrequencyFactorDto>
                .Where(mf => mf.PremiumFrequency, Op.Eq, premiumFrequency)
                .And(mf => mf.BrandId, Op.Eq, brandId);

            var results = _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), premiumFrequency.ToString(),
                () => Query(q));

            return results.FirstOrDefault();
        }
    }
}
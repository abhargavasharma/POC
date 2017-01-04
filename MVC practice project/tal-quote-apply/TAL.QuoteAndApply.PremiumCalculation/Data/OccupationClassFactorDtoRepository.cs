using System;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Models;

namespace TAL.QuoteAndApply.PremiumCalculation.Data
{
    public interface IOccupationClassFactorDtoRepository
    {
        OccupationClassFactorDto GetOccupationClassFactorByGenderOccupationClassAndPlan(Gender gender,
            string occupationClass, string planCode, int brandId);
    }

    public class OccupationClassFactorDtoRepository : BaseRepository<OccupationClassFactorDto>, IOccupationClassFactorDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;

        public OccupationClassFactorDtoRepository(IPremiumCalculationConfigurationProvider setting, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper)
            : base(setting.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
        }

        public OccupationClassFactorDto GetOccupationClassFactorByGenderOccupationClassAndPlan(Gender gender, string occupationClass,
            string planCode, int brandId)
        {
            var q = DbItemPredicate<OccupationClassFactorDto>
               .Where(factor => factor.Gender, Op.Eq, gender)
               .And(factor => factor.OccupationClass, Op.Eq, occupationClass)
               .And(factor => factor.PlanCode, Op.Eq, planCode)
               .And(factor => factor.BrandId, Op.Eq, brandId);

            return _cachingWrapper.GetOrAddCacheItemIndefinite(GetType(), q.PredicateKey, () => Query(q)).FirstOrDefault();
        }
    }
}
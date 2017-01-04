using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface ICoverExclusionDtoRepository
    {
        void RemoveAllExclusionsForCover(int coverId);
        void InsertExclusions(IEnumerable<CoverExclusionDto> coverExclusions);
        IEnumerable<CoverExclusionDto> GetAllExclusionsForCover(int coverId);
    }

    public class CoverExclusionDtoRepository : BaseRepository<CoverExclusionDto>, ICoverExclusionDtoRepository
    {
        private readonly ICoverExclusionChangeSubject _changeSubject;

        public CoverExclusionDtoRepository(IPolicyConfigurationProvider policyConfigurationProvider, ICurrentUserProvider currentUserProvider, IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICoverExclusionChangeSubject changeSubject) 
            : base(policyConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _changeSubject = changeSubject;
        }

        public void RemoveAllExclusionsForCover(int coverId)
        {
            var predicate = DbItemPredicate<CoverExclusionDto>
               .Where(cl => cl.CoverId, Op.Eq, coverId);

            Delete(predicate);
            _changeSubject.Notify(new ChangeEnvelope(new CoverExclusionsChangeItem(coverId)));
        }

        public void InsertExclusions(IEnumerable<CoverExclusionDto> coverExclusions)
        {
            if (coverExclusions.Any())
            {
                Insert(coverExclusions);
                _changeSubject.Notify(new ChangeEnvelope(new CoverExclusionsChangeItem(coverExclusions.FirstOrDefault().CoverId)));
            }
        }

        public IEnumerable<CoverExclusionDto> GetAllExclusionsForCover(int coverId)
        {
            return Where(cl => cl.CoverId, Op.Eq, coverId);
        }
    }
}
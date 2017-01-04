using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface ICoverLoadingDtoRepository
    {
        void RemoveAllLoadingsForCover(int coverId);
        void InsertLoadings(IEnumerable<CoverLoadingDto> coverLoadings);
        IEnumerable<CoverLoadingDto> GetAllLoadingsForCover(int coverId);
    }

    public class CoverLoadingDtoRepository : BaseRepository<CoverLoadingDto>, ICoverLoadingDtoRepository
    {
        private readonly ICoverLoadingChangeSubject _changeSubject;

        public CoverLoadingDtoRepository(IPolicyConfigurationProvider policyConfigurationProvider, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICoverLoadingChangeSubject changeSubject) :
            base(policyConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _changeSubject = changeSubject;
        }

        public void RemoveAllLoadingsForCover(int coverId)
        {
            var predicate = DbItemPredicate<CoverLoadingDto>
                .Where(cl => cl.CoverId, Op.Eq, coverId);

            Delete(predicate);
            _changeSubject.Notify(new ChangeEnvelope(new CoverLoadingChangeItem(coverId)));
        }

        public void InsertLoadings(IEnumerable<CoverLoadingDto> coverLoadings)
        {
            if (coverLoadings.Any())
            {
                Insert(coverLoadings);
                _changeSubject.Notify(new ChangeEnvelope(new CoverLoadingChangeItem(coverLoadings.FirstOrDefault().CoverId)));
            }
        }

        public IEnumerable<CoverLoadingDto> GetAllLoadingsForCover(int coverId)
        {
            return Where(cl => cl.CoverId, Op.Eq, coverId);
        }
    }
}

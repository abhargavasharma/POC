using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Monads;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Search.Configuration;

namespace TAL.QuoteAndApply.Search.Data
{
    public interface IAnswerSearchItemRepository
    {
        IEnumerable<AnswerSearchItemDto> GetAll(string indexKey);
        void SaveAll(IEnumerable<AnswerSearchItemDto> items, string indexKey);
    }

    public class AnswerSearchItemRepository : BaseRepository<AnswerSearchItemDto>, IAnswerSearchItemRepository
    {

        public AnswerSearchItemRepository(ISearchConfigurationProvider config,
            ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(config.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public IEnumerable<AnswerSearchItemDto> GetAll(string key)
        {
            var q = DbItemPredicate<AnswerSearchItemDto>.Where(d => d.IndexKey, Op.Eq, key);
            return Query(q);
        }

        public void SaveAll(IEnumerable<AnswerSearchItemDto> items, string key)
        {
            items.Do(i => i.IndexKey = key);
            var existingItems = Where(a => a.IndexKey, Op.Eq, key);
            var newItems = items.Where(
                rItem => !existingItems.Any(dbItem => rItem.ResponseId == dbItem.ResponseId && rItem.ParentId == dbItem.ParentId));
            Insert(newItems);
        }
    }
}

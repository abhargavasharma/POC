using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.DataLayer.Contract
{
    public interface IBulkCrudDbItemRepository<T> where T : DbItem
    {
        void Insert(IEnumerable<T> items);
        IEnumerable<bool> Update(IEnumerable<T> items);
        IEnumerable<T> GetAll();
        IEnumerable<bool> Delete(IEnumerable<T> items);
    }
}
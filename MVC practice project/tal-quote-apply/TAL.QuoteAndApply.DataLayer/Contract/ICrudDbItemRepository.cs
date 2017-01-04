using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.DataLayer.Contract
{
    public interface ICrudDbItemRepository<T> where T : DbItem
    {
        T Get(int id);
        T Insert(T dbItem);
        void Update(T dbItem);
        bool Delete(T dbItem);
    }
}
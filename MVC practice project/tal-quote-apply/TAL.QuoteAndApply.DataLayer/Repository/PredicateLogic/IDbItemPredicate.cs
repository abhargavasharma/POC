using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic
{
    public interface IDbItemPredicate<T>
    {
        string PredicateKey { get; }
    }
}
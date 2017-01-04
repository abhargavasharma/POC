using System;
using System.Linq.Expressions;

namespace TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic
{
    public interface IPredicateOr<T> : IDbItemPredicate<T>
    {
        IPredicateOr<T> Or(Expression<Func<T, object>> property, Op operation, object value, bool not = false);
    }
}
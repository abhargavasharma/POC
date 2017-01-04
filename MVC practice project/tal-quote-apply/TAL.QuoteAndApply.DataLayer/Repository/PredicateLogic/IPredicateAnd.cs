using System;
using System.Linq.Expressions;

namespace TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic
{
    public interface IPredicateAnd<T> : IDbItemPredicate<T>
    {
        IPredicateAnd<T> And(Expression<Func<T, object>> property, Op operation, object value, bool not = false);
    }
}
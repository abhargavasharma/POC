using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DapperExtensions;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic
{
    public class DbItemPredicate<T> where T : DbItem
    {
        internal PredicateGroup PredicateGroup { get; private set; }

        private DbItemPredicate()
        {
            PredicateGroup = new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate>()
            };
        }

        public static IPredicateAndOr<T> Where(Expression<Func<T, object>> property, Op operation, object value, bool not = false)
        {
            var predicate = new PredicateAndOr<T>();
            predicate.Where(property, operation, value, not);
            return predicate;
        }

        public static IPredicateAndOr<T> Empty()
        {
            var predicate = new PredicateAndOr<T>();
            return predicate;
        } 
    }
}
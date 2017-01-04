using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DapperExtensions;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic
{
    public class PredicateAndOr<T> : IPredicateAndOr<T> where T : DbItem
    {
        internal PredicateGroup PredicateGroup { get; private set; }
        internal List<string> PredicateKeyValues { get; private set; }

        internal PredicateAndOr()
        {
            PredicateGroup = new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate>()
            };
            PredicateKeyValues = new List<string>();
        }

        public IPredicateAnd<T> Where(Expression<Func<T, object>> property, Op operation, object value, bool not = false)
        {
            var predicateValue = value;
            if (operation == Op.StartsWith)
            {
                predicateValue = StartsWithValue(value);
            }
            var dapperOperator = GetDapperOperator(operation);
            PredicateGroup.Predicates.Add(Predicates.Field(property, dapperOperator, predicateValue, not));

            PredicateKeyValues.Add($"W[{operation}-{value}]");
            return this;
        }


        public IPredicateAnd<T> And(Expression<Func<T, object>> property, Op operation, object value, bool not = false)
        {
            var predicateValue = value;
            if (operation == Op.StartsWith)
            {
                predicateValue = StartsWithValue(value);
            }
            var dapperOperator = GetDapperOperator(operation);
            PredicateGroup.Predicates.Add(Predicates.Field(property, dapperOperator, predicateValue, not));

            PredicateKeyValues.Add($"A[{operation}-{value}]");
            return this;
        }

        public IPredicateOr<T> Or(Expression<Func<T, object>> property, Op operation, object value, bool not = false)
        {
            PredicateGroup.Operator = GroupOperator.Or;
            var predicateValue = value;
            if (operation == Op.StartsWith)
            {
                predicateValue = StartsWithValue(value);
            }
            var dapperOperator = GetDapperOperator(operation);
            PredicateGroup.Predicates.Add(Predicates.Field(property, dapperOperator, predicateValue, not));

            PredicateKeyValues.Add($"O[{operation}-{value}]");
            return this;
        }

        public string PredicateKey
        {
            get { return string.Join(".", PredicateKeyValues); }
        }

        private Operator GetDapperOperator(Op from)
        {
            switch (from)
            {
                case Op.Eq:
                    return Operator.Eq;
                case Op.Ge:
                    return Operator.Ge;
                case Op.Gt:
                    return Operator.Gt;
                case Op.Le:
                    return Operator.Le;
                case Op.Lt:
                    return Operator.Lt;
                case Op.StartsWith:
                default:
                    return Operator.Like;
            }
        }

        private string StartsWithValue(object value)
        {
            var strValue = value != null ? value.ToString() : string.Empty;
            return strValue + "%";
        }
    }
}


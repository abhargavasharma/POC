using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api.Underwriting
{
    public class ChangedAttributes : Dictionary<string, object>
    {
        /*
            This will add an entry to dictionary where the key is the attribute name and value is the attributes value
        */
        public void Add<T>(Expression<Func<T>> expression)
        {
            //Grab name of attribute and value to add to dictionary
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
                throw new ApplicationException("Only MemeberExpressions allowed");

            var name = memberExpression.Member.Name;
            var value = expression.Compile()();

            Add(name, value);
        }
    }
}
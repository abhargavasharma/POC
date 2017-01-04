using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Infrastructure.Extensions
{
    public static class ExpressionExtensions
    {
        public static TReturn GetValue<TInput, TReturn>(this Expression<Func<TInput, TReturn>> fieldSelector, TInput obj)
        {
            MemberExpression me;
            switch (fieldSelector.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = fieldSelector.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = fieldSelector.Body as MemberExpression;
                    break;
            }
            if (me != null)
            {
                var propInfo = me.Member as PropertyInfo;
                if (propInfo != null)
                {
                    return (TReturn)propInfo.GetValue(obj);
                }
            }
            return default(TReturn);
        }

        public static string GetName<TInput, TReturn>(this Expression<Func<TInput, TReturn>> fieldSelector)
        {
            MemberExpression me;
            switch (fieldSelector.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = fieldSelector.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = fieldSelector.Body as MemberExpression;
                    break;
            }
            if (me != null)
            {
                var propInfo = me.Member as PropertyInfo;
                if (propInfo != null)
                {
                    return propInfo.Name;
                }
            }
            return string.Empty;
        }
        
    }

    public static class DateExtensions
    {
        public static int Age(this DateTime date)
        {
            var todaysDate = DateTime.Today.Date;
            var age = todaysDate.Year - date.Year;
            if (todaysDate < date.AddYears(age)) age--;

            return age;
        }

        public static int AgeNextBirthday(this DateTime date)
        {
            return Age(date) + 1;
        }
    }
}

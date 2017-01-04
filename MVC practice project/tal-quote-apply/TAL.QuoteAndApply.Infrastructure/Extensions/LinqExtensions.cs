using System;
using System.Collections.Generic;

namespace TAL.QuoteAndApply.Infrastructure.Extensions
{
    public static class LinqExtensions
    {
        //"Borrowed" from http://stackoverflow.com/questions/489258/linqs-distinct-on-a-particular-property
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}

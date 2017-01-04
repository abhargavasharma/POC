using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.Language.Flow;

namespace TAL.QuoteAndApply.Tests.Shared
{
    public static class MoqExtenstions
    {
        public static void ReturnsInOrder<T, TResult>(this ISetup<T, TResult> setup, params TResult[] results) where T : class
        {
            setup.Returns(new Queue<TResult>(results).Dequeue);
        }
    }
}

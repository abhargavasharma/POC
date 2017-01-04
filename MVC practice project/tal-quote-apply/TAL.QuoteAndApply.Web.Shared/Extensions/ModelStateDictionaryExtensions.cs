using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;

namespace TAL.QuoteAndApply.Web.Shared.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static Dictionary<string, IEnumerable<string>> ToErrorDictionary(this System.Web.Http.ModelBinding.ModelStateDictionary modelState)
        {
            var dict = modelState
                .Where(x => x.Value.Errors.Any())
                .ToDictionary(
                    x => x.Key.ToCamelCase('.'),
                    x => x.Value.Errors.Select(e => e.ErrorMessage)
                );

            return dict;
        }
    }
}

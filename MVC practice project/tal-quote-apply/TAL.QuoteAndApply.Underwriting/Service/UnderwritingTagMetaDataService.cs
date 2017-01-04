using System;
using System.Collections.Generic;
using System.Linq;

namespace TAL.QuoteAndApply.Underwriting.Service
{
    public interface IUnderwritingTagMetaDataService
    {
        decimal? GetFirstOrDefault(IEnumerable<string> tags, string key, decimal? defValue);
        int? GetFirstOrDefault(IEnumerable<string> tags, string key, int? defValue);
        string GetFirstOrDefault(IEnumerable<string> tags, string key, string defValue);
        bool TagAndValueExists(IEnumerable<string> tags, string key, string expectedValue);
    }

    public class UnderwritingTagMetaDataService : IUnderwritingTagMetaDataService
    {
        public decimal? GetFirstOrDefault(IEnumerable<string> tags, string key, decimal? defValue)
        {
            var value = GetValue(tags, key).ToArray();
            if (value.Any())
            {
                decimal valeOut;
                if (decimal.TryParse(value.First(), out valeOut))
                {
                    return valeOut;
                }
            }
            return defValue;
        }

        public int? GetFirstOrDefault(IEnumerable<string> tags, string key, int? defValue)
        {
            var value = GetValue(tags, key).ToArray();
            if (value.Any())
            {
                int valeOut;
                if (int.TryParse(value.First(), out valeOut))
                {
                    return valeOut;
                }
            }
            return defValue;
        }

        public string GetFirstOrDefault(IEnumerable<string> tags, string key, string defValue)
        {
            var value = GetValue(tags, key).ToArray();
            return value.Any() ? value.FirstOrDefault() : defValue;
        }

        public bool TagAndValueExists(IEnumerable<string> tags, string key, string expectedValue)
        {
            var value = GetValue(tags, key).ToArray();
            return value.Any(v => v.Equals(expectedValue, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> GetValue(IEnumerable<string> tags, string key)
        {
            foreach (var tag in tags)
            {
                var keyWithPipe = key + "|";
                if (tag.StartsWith(keyWithPipe))
                {
                    yield return tag.Replace(keyWithPipe, "");
                }
            }
        }
    }
}
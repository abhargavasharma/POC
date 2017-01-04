using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Product.Contracts
{
    /// <summary>
    /// Require concrete implementation defined in project that refers to this library
    /// </summary>
    public interface IProductBrandSettingsProvider
    {
        string GetSetting(string brand, string key, string defaultValue);
        int GetSetting(string brand, string key, int defaultValue);
        bool GetSetting(string brand, string key, bool defaultValue);
    }
}

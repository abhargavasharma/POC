using System;
using System.Configuration;

namespace TAL.QuoteAndApply.Web.Shared.Configuration
{
    public interface IWebConfiguration
    {
        bool AllowOverrideParameter { get; }
    }

    public class WebConfiguration : IWebConfiguration
    {
        public bool AllowOverrideParameter
        {
            get
            {
                bool outValue;
                if (Boolean.TryParse(ConfigurationManager.AppSettings["AllowOverrideParameter"],
                    out outValue))
                {
                    return outValue;
                }
                return false;
            }
        }
    }
}

using System.Configuration;

namespace TAL.QuoteAndApply.Analytics.Configuration
{
    public interface IAnalyticsConfiguration
    {
        string AdobeTagManagerScriptUrl { get; }
        string TemplateLocation { get; }
    }

    public class AnalyticsConfiguration : IAnalyticsConfiguration
    {
        public string AdobeTagManagerScriptUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["Analytics.AdobeTagManagerScriptUrl"];
            }
        }

        public string TemplateLocation
        {
            get
            {
                return ConfigurationManager.AppSettings["Analytics.TemplateLocation"];
            }
        }
    }
}
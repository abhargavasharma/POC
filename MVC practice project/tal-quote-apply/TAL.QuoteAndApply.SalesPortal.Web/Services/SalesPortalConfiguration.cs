using System.Configuration;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public interface ISalesPortalConfiguration
    {
        bool ValidatePlansInRealTime { get; }
        int SessionTimeout { get; }
    }

    public class SalesPortalConfiguration : ISalesPortalConfiguration
    {
        public bool ValidatePlansInRealTime
        {
            get
            {
                bool result;
                bool.TryParse(ConfigurationManager.AppSettings["SalesPortal.ValidatePlansInRealTime"], out result);
                return result;
            }
        }

        public int SessionTimeout
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["SalesPortal.SessionTimeout"]);
            }
        }
    }
}
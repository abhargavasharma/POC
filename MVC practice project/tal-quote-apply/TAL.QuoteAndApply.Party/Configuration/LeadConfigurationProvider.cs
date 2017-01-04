using System;
using System.Configuration;

namespace TAL.QuoteAndApply.Party.Configuration
{
    public interface ILeadConfigurationProvider
    {
        string ConnectionString { get; }
        string Version { get; }
        int? Timeout { get; }
        string BrandCode { get; }
    }

    public class LeadConfigurationProvider : ILeadConfigurationProvider
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["Leads.ServiceBaseUrl"]; }
        }

        public string Version
        {
            get { return ConfigurationManager.AppSettings["Leads.Version"]; }
        }

        public int? Timeout
        {
            get { return int.Parse(ConfigurationManager.AppSettings["Leads.Timeout"]); }
        }

        public string BrandCode
        {
            get { return ConfigurationManager.AppSettings["Leads.BrandCode"]; }
        }
    }
}

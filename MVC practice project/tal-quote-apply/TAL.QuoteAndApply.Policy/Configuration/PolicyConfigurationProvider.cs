
using System;
using System.Configuration;

namespace TAL.QuoteAndApply.Policy.Configuration
{
    public interface IPolicyConfigurationProvider
    {
        string ConnectionString { get; }
        bool AccessControlSessionStorage { get; }
    }

    public class PolicyConfigurationProvider : IPolicyConfigurationProvider
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["Policy.SqlConnectionString"]; }
        }

        public bool AccessControlSessionStorage
        {
            get
            {
                return "true".Equals(ConfigurationManager.AppSettings["Policy.AccessControlSessionStorage"], StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}

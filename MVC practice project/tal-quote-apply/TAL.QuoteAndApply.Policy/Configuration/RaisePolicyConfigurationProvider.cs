
using System.Configuration;

namespace TAL.QuoteAndApply.Policy.Configuration
{
    public interface IRaisePolicyConfigurationProvider
    {
        string ConnectionString { get; }
        string UserName { get; }
        string Password { get; }
        string Domain { get; }
        string EnvironmentKey { get; }
        string ErrorNotificationEmailAddress { get; }
    }

    public class RaisePolicyConfigurationProvider : IRaisePolicyConfigurationProvider
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["RaisePolicy.ServiceBaseUrl"]; }
        }

        public string UserName { get { return ConfigurationManager.AppSettings["RaisePolicy.UserName"]; } }
        public string Password { get { return ConfigurationManager.AppSettings["RaisePolicy.Password"]; } }
        public string Domain { get { return ConfigurationManager.AppSettings["RaisePolicy.Domain"]; } }
        public string EnvironmentKey { get { return ConfigurationManager.AppSettings["RaisePolicy.EnvironmentKey"]; } }
        public string ErrorNotificationEmailAddress { get { return ConfigurationManager.AppSettings["RaisePolicy.ErrorNotificationEmailAddress"]; } }
    }
}



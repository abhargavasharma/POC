using System.Configuration;
using TAL.QuoteAndApply.UserRoles.Configuration;

namespace TAL.QuoteAndApply.SalesPortal.Web.Configuration
{
    public class BrandConfigurationElement : ConfigurationElement, IBrandConfiguration
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string BrandKey
        {
            get
            {
                return this["key"] as string;
            }
        }

        [ConfigurationProperty("enabled", IsRequired = true)]
        public bool Enabled
        {
            get
            {
                return (bool)this["enabled"];
            }
        }

        public IBrandRoleSettings RoleSettings
        {
            get { return AuthSetting; }
        }

        [ConfigurationProperty("authSetting")]
        public AuthSetting AuthSetting
        {
            get
            {
                return this["authSetting"] as AuthSetting;
            }
        }
    }
}
using System.Configuration;
using TAL.QuoteAndApply.UserRoles.Configuration;

namespace TAL.QuoteAndApply.SalesPortal.Web.Configuration
{
    public class AuthSetting : ConfigurationElement, IBrandRoleSettings
    {
        [ConfigurationProperty("underwritingGroup", IsRequired = true)]
        public string UnderwritingGroup {
            get
            {
                return this["underwritingGroup"] as string;
            }
        }
        [ConfigurationProperty("agentGroup", IsRequired = true)]
        public string AgentGroup {
            get
            {
                return this["agentGroup"] as string;
            }
        }
        [ConfigurationProperty("readOnlyGroup", IsRequired = true)]
        public string ReadOnlyGroup {
            get
            {
                return this["readOnlyGroup"] as string;
            }
        }
    }

    public class ProductConfigSettings : ConfigurationElement
    {
        [ConfigurationProperty("paymentTypes", IsRequired = true)]
        public string PaymentTypes
        {
            get
            {
                return this["paymentTypes"] as string;
            }
        }
        
    }
}
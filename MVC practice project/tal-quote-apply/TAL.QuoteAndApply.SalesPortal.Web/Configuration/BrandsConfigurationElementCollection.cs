using System.Configuration;

namespace TAL.QuoteAndApply.SalesPortal.Web.Configuration
{
    public class BrandsConfigurationElementCollection
        : ConfigurationElementCollection
    {
        public BrandConfigurationElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as BrandConfigurationElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new BrandConfigurationElement this[string responseString]
        {
            get { return (BrandConfigurationElement)BaseGet(responseString); }
            set
            {
                if (BaseGet(responseString) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
                }
                BaseAdd(value);
            }
        }

        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new BrandConfigurationElement();
        }

        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            return ((BrandConfigurationElement)element).BrandKey;
        }
    }
}
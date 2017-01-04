namespace TeamCity.Client.Model
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class builds
    {

        private buildsBuild[] buildField;

        private byte countField;

        private string hrefField;

        private string nextHrefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("build")]
        public buildsBuild[] build
        {
            get
            {
                return this.buildField;
            }
            set
            {
                this.buildField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href
        {
            get
            {
                return this.hrefField;
            }
            set
            {
                this.hrefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nextHref
        {
            get
            {
                return this.nextHrefField;
            }
            set
            {
                this.nextHrefField = value;
            }
        }
    }
}
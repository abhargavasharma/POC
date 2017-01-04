namespace TeamCity.Client.Model
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class changes
    {

        private changesChange[] changeField;

        private byte countField;

        private string hrefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("change")]
        public changesChange[] change
        {
            get
            {
                return this.changeField;
            }
            set
            {
                this.changeField = value;
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
    }
}
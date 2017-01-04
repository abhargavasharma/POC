namespace TeamCity.Client.Model
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class changeFile
    {

        private string beforerevisionField;

        private string afterrevisionField;

        private string fileField;

        private string relativefileField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("before-revision")]
        public string beforerevision
        {
            get
            {
                return this.beforerevisionField;
            }
            set
            {
                this.beforerevisionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("after-revision")]
        public string afterrevision
        {
            get
            {
                return this.afterrevisionField;
            }
            set
            {
                this.afterrevisionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string file
        {
            get
            {
                return this.fileField;
            }
            set
            {
                this.fileField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("relative-file")]
        public string relativefile
        {
            get
            {
                return this.relativefileField;
            }
            set
            {
                this.relativefileField = value;
            }
        }
    }
}
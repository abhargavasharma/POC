namespace TeamCity.Client.Model
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class change
    {

        private string commentField;

        private changeFile[] filesField;

        private changeVcsRootInstance vcsRootInstanceField;

        private int idField;

        private string versionField;

        private string usernameField;

        private string dateField;

        private string hrefField;

        private string webUrlField;

        /// <remarks/>
        public string comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("file", IsNullable = false)]
        public changeFile[] files
        {
            get
            {
                return this.filesField;
            }
            set
            {
                this.filesField = value;
            }
        }

        /// <remarks/>
        public changeVcsRootInstance vcsRootInstance
        {
            get
            {
                return this.vcsRootInstanceField;
            }
            set
            {
                this.vcsRootInstanceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string username
        {
            get
            {
                return this.usernameField;
            }
            set
            {
                this.usernameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
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
        public string webUrl
        {
            get
            {
                return this.webUrlField;
            }
            set
            {
                this.webUrlField = value;
            }
        }
    }
}
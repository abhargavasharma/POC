namespace TeamCity.Client.Model
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class buildsBuild
    {

        private int idField;

        private string buildTypeIdField;

        private string numberField;

        private string statusField;

        private string stateField;

        private string branchNameField;

        private bool defaultBranchField;

        private string hrefField;

        private string webUrlField;

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
        public string buildTypeId
        {
            get
            {
                return this.buildTypeIdField;
            }
            set
            {
                this.buildTypeIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string branchName
        {
            get
            {
                return this.branchNameField;
            }
            set
            {
                this.branchNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool defaultBranch
        {
            get
            {
                return this.defaultBranchField;
            }
            set
            {
                this.defaultBranchField = value;
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
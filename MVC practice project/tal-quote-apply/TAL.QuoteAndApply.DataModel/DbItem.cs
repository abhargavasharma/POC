using System;

namespace TAL.QuoteAndApply.DataModel
{
    public abstract class DbItem : IDbItem
    {
        public int Id { get; set; }
        public byte[] RV { get; set; }

        public DateTime CreatedTS { get; set; }
        public string CreatedBy { get; set; }

        public DateTime ModifiedTS { get; set; }
        public string ModifiedBy { get; set; }

        public bool? IsEncrypted { get; set; }
    }
}
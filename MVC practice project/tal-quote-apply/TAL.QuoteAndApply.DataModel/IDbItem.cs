using System;

namespace TAL.QuoteAndApply.DataModel
{
    public interface IDbItem
    {
        int Id { get; set; }
        byte[] RV { get; set; }
        DateTime CreatedTS { get; set; }
        string CreatedBy { get; set; }
        DateTime ModifiedTS { get; set; }
        string ModifiedBy { get; set; }
    }
}
namespace TAL.QuoteAndApply.DataModel.Payment
{
    public interface ISelfManagedSuperFund
    {
        string AccountName { get; set; }
        string BSBNumber { get; set; }
        string AccountNumber { get; set; }
    }
}
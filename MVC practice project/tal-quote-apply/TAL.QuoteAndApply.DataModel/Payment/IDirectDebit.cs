namespace TAL.QuoteAndApply.DataModel.Payment
{
    public interface IDirectDebit
    {
        string AccountName { get; set; }
        string BSBNumber { get; set; }
        string AccountNumber { get; set; }
    }
}
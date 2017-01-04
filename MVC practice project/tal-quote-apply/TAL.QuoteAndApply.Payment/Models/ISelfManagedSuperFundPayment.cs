namespace TAL.QuoteAndApply.Payment.Models
{
    public interface ISelfManagedSuperFundPayment : IPayment
    {
        string AccountName { get; set; }
        string BSBNumber { get; set; }
        string AccountNumber { get; set; }
        int Id { get; set; }
    }
}
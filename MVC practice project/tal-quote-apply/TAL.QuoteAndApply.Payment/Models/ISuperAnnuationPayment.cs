namespace TAL.QuoteAndApply.Payment.Models
{
    public interface ISuperAnnuationPayment : IPayment
    {
        string FundName { get; set; }
        string FundABN { get; set; }
        string FundUSI { get; set; }
        string FundProduct { get; set; }
        string MembershipNumber { get; set; }
        string TaxFileNumber { get; set; }
        int Id { get; set; }
    }
}
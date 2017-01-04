namespace TAL.QuoteAndApply.DataModel.Payment
{
    public interface ISuperAnnuationFund
    {
        string FundName { get; set; }
        string FundABN { get; set; }
        string FundUSI { get; set; }
        string FundProduct { get; set; }
        string MembershipNumber { get; set; }
    }
}
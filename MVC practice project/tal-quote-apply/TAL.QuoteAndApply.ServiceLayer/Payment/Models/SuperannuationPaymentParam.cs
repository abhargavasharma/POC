namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models
{
    public class SuperannuationPaymentParam
    {
        public string FundUSI { get; set; }
        public string FundName { get; set; }
        public string FundABN { get; set; }
        public string FundProduct { get; set; }
        public string MembershipNumber { get; set; }
        public string TaxFileNumber { get; set; }

        public bool IsValidForInforce { get; set; }
    }
}
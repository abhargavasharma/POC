namespace TAL.QuoteAndApply.PremiumCalculation.Models
{
    public class PremiumAndDiscount
    {
        public decimal Premium { get; }
        public decimal Discount { get; }

        public PremiumAndDiscount(decimal premium, decimal discount)
        {
            Premium = premium;
            Discount = discount;
        }
    }
}
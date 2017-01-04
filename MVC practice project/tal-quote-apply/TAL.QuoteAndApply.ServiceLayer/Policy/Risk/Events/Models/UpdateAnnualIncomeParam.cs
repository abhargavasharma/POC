namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models
{
    public class UpdateAnnualIncomeParam
    {
        public int RiskId { get; private set; }
        public int PartyId { get; private set; }
        public long AnnualIncome { get; private set; }

        public UpdateAnnualIncomeParam(int riskId, int partyId, long annualIncome)
        {
            RiskId = riskId;
            PartyId = partyId;
            AnnualIncome = annualIncome;
        }
    }
}

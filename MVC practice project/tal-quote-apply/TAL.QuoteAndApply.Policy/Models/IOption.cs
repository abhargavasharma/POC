namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IOption
    {
        int Id { get; }
        int RiskId { get; set; }
        string Code { get; set; }
        bool? Selected { get; set; }
        int PlanId { get; set; }

    }
}
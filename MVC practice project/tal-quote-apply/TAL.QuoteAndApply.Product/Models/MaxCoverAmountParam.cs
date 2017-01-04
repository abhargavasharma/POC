namespace TAL.QuoteAndApply.Product.Models
{
    public interface IMaxCoverAmountParam
    {
        string PlanCode { get; }
        string BrandKey { get; }
        int Age { get; }
        long AnnualIncome { get; }
        int CoverAmount { get; }
        int? ParentPlanCoverCap { get; }
    }

    public class MaxCoverAmountParam : IMaxCoverAmountParam
    {
        public string PlanCode { get;  }
        public string BrandKey { get; }
        public int Age { get; }
        public long AnnualIncome { get; }
        public int CoverAmount { get; }
        public int? ParentPlanCoverCap { get; }

        public MaxCoverAmountParam(string planCode, string brandKey, int age, long annualIncome, int coverAmount, int? parentPlanCoverCap)
        {
            PlanCode = planCode;
            BrandKey = brandKey;
            Age = age;
            AnnualIncome = annualIncome;
            CoverAmount = coverAmount;
            ParentPlanCoverCap = parentPlanCoverCap;
        }
    }
}

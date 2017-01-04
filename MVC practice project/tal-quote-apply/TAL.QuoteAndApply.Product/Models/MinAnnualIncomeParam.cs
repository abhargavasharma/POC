
namespace TAL.QuoteAndApply.Product.Models
{
    public class MinAnnualIncomeParam
    {
        public string PlanCode { get; private set; }
        public int Age { get; private set; }

        public MinAnnualIncomeParam(string planCode, int age)
        {
            PlanCode = planCode;
            Age = age;
        }
    }
}

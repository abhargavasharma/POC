
using TAL.QuoteAndApply.Product.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Models.Converters
{
    public interface IMinAnnualIncomeParamConverter
    {
        MinAnnualIncomeParam CreateFrom(PlanStateParam planOptionsParam);
    }

    public class MinAnnualIncomeParamConverter : IMinAnnualIncomeParamConverter
    {
        public MinAnnualIncomeParam CreateFrom(PlanStateParam planOptionsParam)
        {
            return new MinAnnualIncomeParam(planOptionsParam.PlanCode, planOptionsParam.Age);
        }
    }
}

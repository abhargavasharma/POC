using System;
using System.Configuration;

namespace TAL.QuoteAndApply.PremiumCalculation.Configuration
{
    public interface IPremiumCalculationConfigurationProvider
    {
        string ConnectionString { get; }
        int MultiPlanDiscountPlanLimit { get; }
    }

    public class PremiumCalculationConfigurationProvider : IPremiumCalculationConfigurationProvider
    {
        public string ConnectionString => ConfigurationManager.AppSettings["PremiumCalculation.SqlConnectionString"];
        public int MultiPlanDiscountPlanLimit => Convert.ToInt32(ConfigurationManager.AppSettings["PremiumCalculation.MultiPlanDiscountPlanLimit"]);
    }
}

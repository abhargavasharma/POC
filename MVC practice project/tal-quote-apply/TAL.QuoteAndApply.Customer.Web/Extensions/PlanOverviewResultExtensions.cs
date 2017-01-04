using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.Extensions
{
    public static class PlanOverviewResultExtensions
    {
        public static PremiumType DefaultCustomerPremiumType(this IEnumerable<PlanOverviewResult> planOverviewResults)
        {
            //PremiumType is per plan in Sales Portal but per policy/risk in Customer Site, currently just taking the Premium Type of first plan.
            //(Only an issue if Sales Agent has fiddled around in Sales Portal)
            //TODO: Putting this here as a common convienient place to call but could move into service layer or somewhere more official if needed
            return planOverviewResults.First().PremiumType;
        }
    }
}
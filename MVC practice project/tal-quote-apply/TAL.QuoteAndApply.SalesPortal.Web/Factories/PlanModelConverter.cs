using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Factories
{
    public interface IPlanModelConverter
    {
        AvailablePlanOptions GetAvailablePlanOptions(AvailablePlanOptionsAndConfigResult item);
        AvailableRiderOptions GetAvailablePlanOptions(AvailableRiderOptionsAndConfigResult item);
        AvailabilityPlanStateParam GetSelectedPlanOptionsParam(int riskId, string brandKey, SelectedPlanOptions item);
    }

    public class PlanModelConverter : IPlanModelConverter
    {
        public AvailablePlanOptions GetAvailablePlanOptions(AvailablePlanOptionsAndConfigResult item)
        {
            var retVal = new AvailablePlanOptions()
            {
                PlanCode = item.PlanCode,
                AvailableCovers = item.AvailableCovers.ToList(),
                AvailablePlans = item.AvailablePlans.ToList(),
                AvailableOptions = item.AvailableOptions.ToList(),
                AvailableRiders = item.AvailableRiders.Select(GetAvailablePlanOptions).ToList()
            };
            return retVal;
        }

        public AvailableRiderOptions GetAvailablePlanOptions(AvailableRiderOptionsAndConfigResult item)
        {
            var retVal = new AvailableRiderOptions()
            {
                AvailableCovers = item.With(i => i.AvailableCovers).Return(ac => ac.ToList(), null),
                AvailableOptions = item.With(i => i.AvailableOptions).Return(ao => ao.ToList(), null),
                RiderCode = item.RiderCode
            };
            return retVal;
        }

        public AvailabilityPlanStateParam GetSelectedPlanOptionsParam(int riskId, string brandKey, SelectedPlanOptions item)
        {
            var retVal = new AvailabilityPlanStateParam()
            {
                PlanCode = item.PlanCode,
                BrandKey = brandKey,
                RiskId = riskId,
                SelectedCoverCodes = item.SelectedCovers.ToList(),
                SelectedPlanCodes = item.SelectedPlans.ToList(),
                SelectedRiderCoverCodes = item.SelectedRiderCovers.ToList(),
                SelectedRiderCodes = item.SelectedRiders.Where(r => r.Selected).Select(r => r.PlanCode).ToList()
            };
            return retVal;
        }
    }
}
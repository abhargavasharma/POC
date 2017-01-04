using System.Linq;
using Status = TAL.QuoteAndApply.DataModel.Policy.MarketingStatus;

namespace TAL.QuoteAndApply.Policy.Service.MarketingStatus
{
    public interface IRiskMarketingStatusProvider
    {
        Status GetRiskMarketingStatus(Status[] plansMarketingStatusList);
    }
    public class RiskMarketingStatusProvider : IRiskMarketingStatusProvider
    {
        public Status GetRiskMarketingStatus(Status[] plansMarketingStatusList)
        {
            var marketingStatus = Status.Off;
            var planBlockSelection = Status.Lead;
            foreach (var plan in plansMarketingStatusList.Where(plan => plan != Status.Off))
            {
                planBlockSelection = UpdatePlanBlockSelection(planBlockSelection, plan, plansMarketingStatusList);
                marketingStatus = planBlockSelection >= marketingStatus || marketingStatus == Status.Decline || marketingStatus == Status.Off ? planBlockSelection : Status.Lead;
            }
            return marketingStatus;
        }

        private Status UpdatePlanBlockSelection(Status planBlockSelection, Status plan, Status[] planMarketingStatusList)
        {
            switch (plan)
            {
                case Status.Refer:
                    return Status.Refer;
                case Status.Ineligible:
                    if (NoLeadAcceptOrReferStatusesInCoverBlockSelection(planMarketingStatusList)) { return Status.Ineligible; }
                    break;
                case Status.Decline:
                    if (NoLeadAcceptOrReferStatusesInCoverBlockSelection(planMarketingStatusList) && !planMarketingStatusList.Contains(Status.Off)) { return Status.Decline; }
                    break;
            }
            return planBlockSelection;
        }

        private bool NoLeadAcceptOrReferStatusesInCoverBlockSelection(Status[] coversMarketingStatusList)
        {
            //This is for the case when a lead, accept or refer status on a cover would override either an ineligible or decline total coverBlock selection 
            return !coversMarketingStatusList.Contains(Status.Lead) &&
                   !coversMarketingStatusList.Contains(Status.Accept) &&
                   !coversMarketingStatusList.Contains(Status.Refer);
        }
    }
}

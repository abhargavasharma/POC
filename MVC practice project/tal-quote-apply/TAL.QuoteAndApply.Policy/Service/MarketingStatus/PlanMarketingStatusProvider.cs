using System.Linq;
using Status = TAL.QuoteAndApply.DataModel.Policy.MarketingStatus;

namespace TAL.QuoteAndApply.Policy.Service.MarketingStatus
{
    public interface IPlanMarketingStatusProvider
    {
        Status GetPlanMarketingStatus(bool selected, bool planEligibile, Status[] coversMarketingStatusList);
    }

    public class PlanMarketingStatusProvider : IPlanMarketingStatusProvider
    {
        public Status GetPlanMarketingStatus(bool selected, bool planEligibile, Status[] coversMarketingStatusList)
        {
            if (!selected)
            {
                return Status.Off;
            }

            if (!planEligibile)
            {
                return Status.Ineligible;
            }

            var marketingStatus = Status.Lead;
            //CoverBlockSelection is the incremental formation of the total plan status as determined when looping through the cover statuses
            var coverBlockSelection = Status.Lead;

            foreach (var cover in coversMarketingStatusList.Where(cover => cover != Status.Off))
            {
                coverBlockSelection = UpdateCoverBlockSelection(coverBlockSelection, cover, coversMarketingStatusList);
                if (coverBlockSelection > marketingStatus)
                {
                    marketingStatus = coverBlockSelection;
                }
            }
            
            return marketingStatus;
        }

        private Status UpdateCoverBlockSelection(Status coverBlockSelection, Status cover, Status[] coversMarketingStatusList)
        {
            switch (cover)
            {
                case Status.Refer:
                    return Status.Refer;
                case Status.Ineligible:
                    if (NoLeadAcceptOrReferStatusesInCoverBlockSelection(coversMarketingStatusList)) { return Status.Ineligible; }
                    break;
                case Status.Decline:
                    if (NoLeadAcceptOrReferStatusesInCoverBlockSelection(coversMarketingStatusList) && !coversMarketingStatusList.Contains(Status.Off)){ return Status.Decline; }
                    return Status.Refer;
            }
            return coverBlockSelection;
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
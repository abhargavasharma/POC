using TAL.QuoteAndApply.DataModel.Underwriting;
using Status = TAL.QuoteAndApply.DataModel.Policy.MarketingStatus;

namespace TAL.QuoteAndApply.Policy.Service.MarketingStatus
{
    public interface ICoverMarketingStatusProvider
    {
        Status GetCoverMarketingStatus(bool selected, bool eligibile, UnderwritingStatus underwritingStatus);
    }

    public class CoverMarketingStatusProvider : ICoverMarketingStatusProvider
    {
        public Status GetCoverMarketingStatus(bool selected, bool eligibile, UnderwritingStatus underwritingStatus)
        {
            if (!selected)
            {
                return Status.Off;
            }

            if (underwritingStatus == UnderwritingStatus.Decline)
            {
                return Status.Decline;
            }

            if (!eligibile)
            {
                return Status.Ineligible;
            }
            return AssignEligibleCoverMarketingStatus(underwritingStatus);
        }

        private Status AssignEligibleCoverMarketingStatus(UnderwritingStatus underwritingStatus)
        {
            switch (underwritingStatus)
            {
                case UnderwritingStatus.Refer:
                case UnderwritingStatus.Defer:
                    return Status.Refer;
                case UnderwritingStatus.Accept:
                    return Status.Accept;
            }
            return Status.Lead;
        }
    }
}
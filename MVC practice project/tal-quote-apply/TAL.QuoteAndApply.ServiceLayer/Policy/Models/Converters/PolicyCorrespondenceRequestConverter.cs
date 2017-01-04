using System.Collections.Generic;
using TAL.QuoteAndApply.Notifications.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IPolicyCorrespondenceRequestConverter
    {
        EmailQuoteSavedViewModel From(PolicyCorrespondenceRequest policyCorrespondenceRequest);
    }

    public class PolicyCorrespondenceRequestConverter : IPolicyCorrespondenceRequestConverter
    {
        public EmailQuoteSavedViewModel From(PolicyCorrespondenceRequest policyCorrespondenceRequest)
        {
            var planSummaries = new List<EmailQuoteSavedPlanSummary>();
            var premiumTotal = 0.0m;
            foreach (var plan in policyCorrespondenceRequest.PlanSummaries)
            {
                premiumTotal += plan.Premium;
                var coverSummaries = new List<EmailQuoteSavedCoverSummary>();
                foreach (var cover in plan.CoverCorrespondenceSummaries)
                {
                    coverSummaries.Add(new EmailQuoteSavedCoverSummary()
                    {
                        Name = cover.Name
                    });
                }
                planSummaries.Add(new EmailQuoteSavedPlanSummary()
                {
                    CoverAmount = $"{plan.CoverAmount:n}",
                    Name = plan.Name,
                    Premium = $"{plan.Premium:n}",
                    CoverSummaries = coverSummaries,
                    IsRider = plan.IsRider,
                    ParentName = plan.ParentName
                });
            }
            var returnObj = new EmailQuoteSavedViewModel()
            {
                AgentEmailAddress = policyCorrespondenceRequest.UserEmailAddress,
                BaseEmailModel = new EmailQuoteViewModel()
                {
                    EmailAddress = policyCorrespondenceRequest.ClientEmailAddress,
                    FirstName = policyCorrespondenceRequest.ClientFirstName
                },
                FullAgentName = policyCorrespondenceRequest.UserFullName,
                PlanSummaries = planSummaries,
                PremiumTotal = $"{premiumTotal:n}",
                PremiumFrequency = policyCorrespondenceRequest.PremiumFrequency,
                Brandkey = policyCorrespondenceRequest.BrandKey
            };
            return returnObj;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Notifications.Models;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using CoverCorrespondenceSummaryViewModel = TAL.QuoteAndApply.SalesPortal.Web.Models.Api.CoverCorrespondenceSummaryViewModel;
using PlanCorrespondenceSummaryViewModel = TAL.QuoteAndApply.SalesPortal.Web.Models.Api.PlanCorrespondenceSummaryViewModel;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPolicyCorrespondenceResultConverter
    {
        PolicyCorrespondenceViewModel From(RiskPlanDetailReposone riskPlanDetailReposone, string userEmailAddress, SaveQuoteEmailRequest saveQuoteEmailRequest);

        PolicyCorrespondenceRequest ToPolicyCorrespondenceRequest(ICurrentUser currentUser,
            RiskPlanDetailReposone riskPlanDetailReposone, SaveQuoteEmailRequest saveQuoteEmailRequest, string brandKey);
    }

    public class PolicyCorrespondenceResultConverter : IPolicyCorrespondenceResultConverter
    {
        public PolicyCorrespondenceViewModel From(RiskPlanDetailReposone riskPlanDetailReposone, string userEmailAddress, SaveQuoteEmailRequest saveQuoteEmailRequest)
        {
            var planSummaries = new List<PlanCorrespondenceSummaryViewModel>();
            foreach (var plan in riskPlanDetailReposone.Plans)
            {
                if (!plan.IsRider && plan.Selected)
                {
                    planSummaries.Add(new PlanCorrespondenceSummaryViewModel()
                    {
                        Name = string.Concat(plan.Name, " Cover"),
                        CoverAmount = plan.CoverAmount,
                        Premium = plan.Premium,
                        IsRider = false,
                        CoverCorrespondenceSummaries =
                            plan.Covers.Where(c => c.Selected).Select(y => new CoverCorrespondenceSummaryViewModel()
                            {
                                Name = y.Name,
                                Selected = y.Selected
                            }).ToList()
                    });
                    planSummaries.AddRange(plan.Riders.Where(r => r.Selected).Select(rider => new PlanCorrespondenceSummaryViewModel()
                    {
                        Name = string.Concat(rider.Name, " Cover"), 
                        CoverAmount = rider.CoverAmount,
                        Premium = rider.Premium,
                        IsRider = true,
                        ParentName = plan.ShortName,
                        CoverCorrespondenceSummaries =
                            rider.Covers.Where(c => c.Selected).Select(y => new CoverCorrespondenceSummaryViewModel()
                            {
                                Name = y.Name,
                                Selected = y.Selected
                            }).ToList()
                    }));
                }
            }

            return new PolicyCorrespondenceViewModel()
            {
                CustomerEmailAddress = saveQuoteEmailRequest.ClientEmailAddress,
                ClientFullName = saveQuoteEmailRequest.ClientFullName,
                UserEmailAddress = userEmailAddress,
                PlanSummaries = planSummaries,
                IsValidForEmailCorrespondence = saveQuoteEmailRequest.IsValidForEmailCorrespondence,
                PremiumFrequency = GetCorrespondencePremiumFrequency(riskPlanDetailReposone.Plans.First().PremiumFrequency)
            };
        }

        public PolicyCorrespondenceRequest ToPolicyCorrespondenceRequest(ICurrentUser currentUser,
            RiskPlanDetailReposone riskPlanDetailReposone, SaveQuoteEmailRequest saveQuoteEmailRequest, string brandKey)
        {
            var planSummaries = new List<PlanCorrespondenceSummary>();
            foreach (var plan in riskPlanDetailReposone.Plans)
            {
                if (!plan.IsRider && plan.Selected)
                {
                    planSummaries.Add(new PlanCorrespondenceSummary()
                    {
                        Name = string.Concat(plan.Name, " Cover"),
                        CoverAmount = plan.CoverAmount,
                        Premium = plan.Premium,
                        IsRider = false,
                        CoverCorrespondenceSummaries =
                            plan.Covers.Where(c => c.Selected).Select(y => new CoverCorrespondenceSummary()
                            {
                                Name = y.Name,
                                Selected = y.Selected
                            }).ToList()
                    });
                    planSummaries.AddRange(plan.Riders.Where(r => r.Selected).Select(rider => new PlanCorrespondenceSummary()
                    {
                        Name = string.Concat(rider.Name, " Cover"),
                        CoverAmount = rider.CoverAmount,
                        Premium = rider.Premium,
                        IsRider = true,
                        ParentName = plan.ShortName,
                        CoverCorrespondenceSummaries =
                            rider.Covers.Where(c => c.Selected).Select(y => new CoverCorrespondenceSummary()
                            {
                                Name = y.Name,
                                Selected = y.Selected
                            }).ToList()
                    }));
                }
            }

            return new PolicyCorrespondenceRequest()
            {
                ClientEmailAddress = saveQuoteEmailRequest.ClientEmailAddress,
                ClientFirstName = saveQuoteEmailRequest.ClientFirstName,
                UserEmailAddress = currentUser.EmailAddress,
                UserFullName = string.Concat(currentUser.GivenName, " ", currentUser.Surname),
                PlanSummaries = planSummaries,
                IsValidForEmailCorrespondence = saveQuoteEmailRequest.IsValidForEmailCorrespondence,
                PremiumFrequency = GetCorrespondencePremiumFrequency(riskPlanDetailReposone.Plans.First().PremiumFrequency),
                BrandKey = brandKey
            };
        }

        public string GetCorrespondencePremiumFrequency(string premiumFrequency)
        {
            switch (premiumFrequency)
            {
                case "Monthly":
                    return "Month";
                case "Quarterly":
                    return "Quarter";
                case "Yearly":
                    return "Year";
                case "HalfYearly":
                    return "Half Year";
                case "Unknown":
                    return "Month"; //No point in showing Unknown if it ever happens
                default:
                    return "Month";
            }
        }
    }
}
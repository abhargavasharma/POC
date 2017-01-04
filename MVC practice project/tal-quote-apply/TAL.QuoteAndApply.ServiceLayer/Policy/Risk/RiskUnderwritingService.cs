using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Models;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk
{
    public interface IRiskUnderwritingService
    {
        RiskUnderwritingCompleteResult GetRiskUnderwritingStatus(int riskId);
        void SetRiskStatusToIncomplete(int riskId);
        void SetRiskStatusToComplete(int riskId);
        void SetRiskStatusToRefer(int riskId);
    }

    public class RiskUnderwritingService : IRiskUnderwritingService
    {
        private readonly IRiskService _riskService;
        private readonly IPlanService _planService;
        private readonly ICoverService _coverService;

        public RiskUnderwritingService(IRiskService riskService, IPlanService planService, ICoverService coverService)
        {
            _riskService = riskService;
            _planService = planService;
            _coverService = coverService;
        }

        public RiskUnderwritingCompleteResult GetRiskUnderwritingStatus(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);

            var coverUwCompleteResults = new List<CoverUnderwritingCompleteResult>();

            if (risk != null)
            {
                var plans = _planService.GetPlansForRisk(risk.Id);

                foreach (var plan in plans.Where(plan => plan.Selected))
                {
                    var covers = _coverService.GetCoversForPlan(plan.Id);

                    foreach (var cover in covers.Where(cover=> cover.Selected))
                    {
                        var coverUwCompleteResult = new CoverUnderwritingCompleteResult(cover.Code, IsUnderwritingCompleteForCover(cover), GetValidationTypeForCover(cover));
                        coverUwCompleteResults.Add(coverUwCompleteResult);
                    }
                }
            }

            return RollupCoverUnderwritingCompleteResultsToRisk(coverUwCompleteResults);
        }

        public void SetRiskStatusToIncomplete(int riskId)
        {
            SetRiskStatus(riskId, InterviewStatus.Incomplete);
        }

        public void SetRiskStatusToComplete(int riskId)
        {
            SetRiskStatus(riskId, InterviewStatus.Complete);
        }

        public void SetRiskStatusToRefer(int riskId)
        {
            SetRiskStatus(riskId, InterviewStatus.Referred);
        }

        private void SetRiskStatus(int riskId, InterviewStatus newStatus)
        {
            var risk = _riskService.GetRisk(riskId);
            risk.InterviewStatus = newStatus;
            _riskService.UpdateRisk(risk);
        }

        private void ForceRiskStatus(int riskId, InterviewStatus interviewStatus)
        {
            var risk = _riskService.GetRisk(riskId);
            risk.InterviewStatus = interviewStatus;
            _riskService.UpdateRisk(risk);
        }

        private RiskUnderwritingCompleteResult RollupCoverUnderwritingCompleteResultsToRisk(IList<CoverUnderwritingCompleteResult> coverUwCompleteResults)
        {
            var isUnderwritingCompleteForRisk = (!coverUwCompleteResults.Any() || coverUwCompleteResults.All(cover => cover.IsUnderwritingComplete));
            var validationTypeForRisk = GetValidaitonTypeForRisk(coverUwCompleteResults.Select(x => x.ValidationType).ToList());

            return new RiskUnderwritingCompleteResult(isUnderwritingCompleteForRisk, validationTypeForRisk, coverUwCompleteResults);
        }

        private ValidationType GetValidaitonTypeForRisk(IList<ValidationType> coverValidationTypes)
        {
            if (coverValidationTypes.Any(x => x == ValidationType.Error))
            {
                return ValidationType.Error;
            }

            if (coverValidationTypes.Any(x => x == ValidationType.Warning))
            {
                return ValidationType.Warning;
            }

            return ValidationType.Success;
        }

        private ValidationType GetValidationTypeForCover(ICover cover)
        {
            switch (cover.UnderwritingStatus)
            {
                case UnderwritingStatus.Decline:
                    return ValidationType.Error;

                case UnderwritingStatus.Accept:
                    return ValidationType.Success;
                
                case UnderwritingStatus.Defer:
                case UnderwritingStatus.Incomplete:
                case UnderwritingStatus.MoreInfo:
                case UnderwritingStatus.Refer:
                    return ValidationType.Warning;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsUnderwritingCompleteForCover(ICover cover)
        {
            return cover.UnderwritingStatus == UnderwritingStatus.Accept;
        }
    }
}

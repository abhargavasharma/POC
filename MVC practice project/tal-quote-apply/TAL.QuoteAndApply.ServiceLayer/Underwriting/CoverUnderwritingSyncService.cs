using System;
using System.Linq;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Converters;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.Underwriting.Models.Event;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public class CoverUnderwritingSyncService : IUnderwritingBenefitsResponseChangeObserver
    {
        private readonly IRiskService _riskService;
        private readonly IPlanService _planService;
        private readonly ICoverService _coverService;
        private readonly ICoverLoadingService _coverLoadingService;
        private readonly ICoverLoadingsConverter _coverLoadingsConverter;
        private readonly IGetUnderwritingBenefitCodeForCoverService _getUnderwritingBenefitCodeForCoverService;
        private readonly ICoverExclusionsConverter _coverExclusionsConverter;
        private readonly ICoverExclusionsService _coverExclusionsService;

        public CoverUnderwritingSyncService(IRiskService riskService, 
            IPlanService planService, 
            ICoverService coverService, 
            ICoverLoadingService coverLoadingService,
            ICoverLoadingsConverter coverLoadingsConverter,
            IGetUnderwritingBenefitCodeForCoverService getUnderwritingBenefitCodeForCoverService, 
            ICoverExclusionsConverter coverExclusionsConverter,
            ICoverExclusionsService coverExclusionsService)
        {
            _riskService = riskService;
            _planService = planService;
            _coverService = coverService;
            _coverLoadingService = coverLoadingService;
            _getUnderwritingBenefitCodeForCoverService = getUnderwritingBenefitCodeForCoverService;
            _coverExclusionsConverter = coverExclusionsConverter;
            _coverExclusionsService = coverExclusionsService;
            _coverLoadingsConverter = coverLoadingsConverter;
        }

        public void Update(UnderwritingBenefitResponsesChangeParam underwritingBenefitResponsesChange)
        {
            var risk = _riskService.GetRiskByInterviewId(underwritingBenefitResponsesChange.InterviewId);

            if (risk != null)
            {
                var plans = _planService.GetPlansForRisk(risk.Id);

                foreach (var plan in plans)
                {
                    var covers = _coverService.GetCoversForPlan(plan.Id);

                    foreach (var cover in covers)
                    {
                        var uwBenefitCodeForCover =
                            _getUnderwritingBenefitCodeForCoverService.GetUnderwritingBenefitCodeFrom(cover);

                        var matchingBenefitResponse =
                            underwritingBenefitResponsesChange.BenefitResponseStatuses.FirstOrDefault(
                                x => x.BenefitCode.Equals(uwBenefitCodeForCover, StringComparison.InvariantCultureIgnoreCase));

                        if (matchingBenefitResponse != null)
                        {
                            cover.UnderwritingStatus = matchingBenefitResponse.UnderwritingStatus;
                            
                            _coverService.UpdateCover(cover);
                            _coverLoadingService.UpdateLoadingsForCover(cover, _coverLoadingsConverter.From(cover, matchingBenefitResponse.TotalLoadings));
                            _coverExclusionsService.UpdateCoverExclusions(cover, _coverExclusionsConverter.From(cover, matchingBenefitResponse.ReadOnlyExclusions));
                        }
                    }
                }
            }
        }
    }
}
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Referral
{
    public interface IAssignReferralService
    {
        ReferralDetailsResult Assign(string quoteReferenceNumber, string userName);
    }

    public class AssignReferralService : IAssignReferralService
    {

        private readonly IReferralService _referralService;
        private readonly IPolicyWithRisksService _policyWithRisksService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IPlanService _planService;
        private readonly IReferralConverter _referralConverter;

        public AssignReferralService(IDateTimeProvider dateTimeProvider, 
            IPolicyWithRisksService policyWithRisksService, 
            IReferralService referralService, 
            IPolicyOverviewProvider policyOverviewProvider, 
            IPlanService planService, IReferralConverter referralConverter)
        {
            _dateTimeProvider = dateTimeProvider;
            _policyWithRisksService = policyWithRisksService;
            _referralService = referralService;
            _policyOverviewProvider = policyOverviewProvider;
            _planService = planService;
            _referralConverter = referralConverter;
        }

        public ReferralDetailsResult Assign(string quoteReferenceNumber, string userName)
        {
            var policyWrapper = _policyWithRisksService.GetFrom(quoteReferenceNumber);

            var policyOverview = _policyOverviewProvider.GetFor(quoteReferenceNumber);
            var plans = _planService.GetPlansForRisk(policyOverview.OwnerRiskId.Value);

            return _referralConverter.From(_referralService.AssignReferralToUnderwriter(policyWrapper.Policy.Id, userName,
                _dateTimeProvider.GetCurrentDateAndTime()), policyOverview, plans);
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Referral
{
    public interface IGetReferralsService
    {
        List<ReferralDetailsResult> GetAll();
    }

    public class GetReferralsService : IGetReferralsService
    {
        private readonly IReferralService _referralService;
        private readonly IReferralConverter _referralConverter;
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IPolicyService _policyService;
        private readonly IPlanService _planService;

        public GetReferralsService(IReferralService referralService, IReferralConverter referralConverter, IPolicyService policyService, IPolicyOverviewProvider policyOverviewProvider, IPlanService planService)
        {
            _referralService = referralService;
            _referralConverter = referralConverter;
            _policyOverviewProvider = policyOverviewProvider;
            _planService = planService;
            _policyService = policyService;
        }

        

        public List<ReferralDetailsResult> GetAll()
        {
            var result = _referralService.GetAll();
            return (from referral in result
                    let policy = _policyService.Get(referral.PolicyId)
                    let policyOverview = _policyOverviewProvider.GetFor(policy.QuoteReference)
                    let ownerRiskId = policyOverview.OwnerRiskId
                where ownerRiskId != null
                where ownerRiskId != null
                    let plans = _planService.GetPlansForRisk(ownerRiskId.Value)
                    select _referralConverter.From(referral, policyOverview, plans)).ToList();
        }
    }
}

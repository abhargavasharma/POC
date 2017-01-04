using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPolicyWithRisksService
    {
        PolicyWithRisks GetFrom(string quoteReferenceNo);
        IEnumerable<int> GetRiskIdsFrom(string quoteReferenceNo);

        /// <summary>
        /// TOOD: this should be removed
        /// </summary>
        /// <param name="quoteReferenceNo"></param>
        /// <returns></returns>
        IRisk GetPolicyOwnderRiskFrom(string quoteReferenceNo);
        void SaveAll(PolicyWithRisks policyWithRisks);
    }

    public class PolicyWithRisksService : IPolicyWithRisksService
    {
        private readonly IPolicyService _policyService;
        private readonly IRiskService _riskService;
        private readonly IPlanService _planService;
        private readonly ICoverService _coverService;
        private readonly IPolicyOwnerService _policyOwnerService;
        private readonly IOptionService _optionsService;
        private readonly IUpdateMarketingStatusService _updateMarketingStatusService;

        public PolicyWithRisksService(
            IPolicyService policyService,
            IRiskService riskService,
            IPlanService planService,
            ICoverService coverService, 
            IPolicyOwnerService policyOwnerService,
            IOptionService optionsService, 
            IUpdateMarketingStatusService updateMarketingStatusService)
        {
            _policyService = policyService;
            _riskService = riskService;
            _planService = planService;
            _coverService = coverService;
            _policyOwnerService = policyOwnerService;
            _optionsService = optionsService;
            _updateMarketingStatusService = updateMarketingStatusService;
        }

        public PolicyWithRisks GetFrom(string quoteReferenceNo)
        {
            var riskWithPlans = new List<RiskWithPlans>();

            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNo);
            var risks = _policyService.GetRisksForPolicy(policy);


            foreach (var risk in risks)
            {
                var planWithCovers = new List<PlanWithCovers>();

                var plans = _planService.GetPlansForRisk(risk.Id);

                foreach (var plan in plans)
                {
                    var covers = _coverService.GetCoversForPlan(plan.Id);
                    var options = _optionsService.GetOptionsForPlan(plan.Id);

                    planWithCovers.Add(new PlanWithCovers(plan, covers, options));
                }

                riskWithPlans.Add(new RiskWithPlans(risk, planWithCovers));
            }
            
            return new PolicyWithRisks(policy, riskWithPlans);
        }
		
		public IEnumerable<int> GetRiskIdsFrom(string quoteReferenceNo)
        {
            var policyWithRisks = GetFrom(quoteReferenceNo);
            return policyWithRisks.Risks.Select(r => r.Risk.Id);
        }

		
        public IRisk GetPolicyOwnderRiskFrom(string quoteReferenceNo)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReferenceNo);
            var policyOwnerId = _policyOwnerService.GetPolicyOwnerPartyId(policy.Id);
            if (policyOwnerId != null)
            {
                return _riskService.GetRiskByPartyId(policyOwnerId.Value);
            }
            return null;   
        }

        public void SaveAll(PolicyWithRisks policyWithRisks)
        {
            foreach (var riskWrapper in policyWithRisks.Risks)
            {
                foreach (var planWrapper in riskWrapper.Plans)
                {
                    foreach (var cover in planWrapper.Covers)
                    {
                        _coverService.UpdateCover(cover);
                    }

                    _planService.UpdatePlan(planWrapper.Plan);
                }

                _riskService.UpdateRisk(riskWrapper.Risk);
                _updateMarketingStatusService.UpdateMarketingStatusForRisk(riskWrapper.Risk.Id);
            }

            _policyService.UpdatePolicy(policyWithRisks.Policy);
        }
    }

    

    public class PlanWithCovers
    {
        public IPlan Plan { get; }
        public IEnumerable<ICover> Covers { get; }
        public IEnumerable<IOption> Options { get; }

        public PlanWithCovers(IPlan plan, IEnumerable<ICover> covers, IEnumerable<IOption> options)
        {
            Plan = plan;
            Covers = covers;
            Options = options;
        }
    }

    public class RiskWithPlans
    {
        public IRisk Risk { get;  }
        public IEnumerable<PlanWithCovers> Plans { get; }

        public RiskWithPlans(IRisk risk, IEnumerable<PlanWithCovers> plans)
        {
            Risk = risk;
            Plans = plans;
        }
    }

    public class PolicyWithRisks
    {
        public IPolicy Policy { get; }
        public IEnumerable<RiskWithPlans> Risks { get; }

        public PolicyWithRisks(IPolicy policy, IEnumerable<RiskWithPlans> risks)
        {
            Policy = policy;
            Risks = risks;
        }
    }
}
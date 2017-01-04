using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Referral;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyFactory
    {
        Models.RaisePolicy GetFromQuoteReference(string quoteReference);
    }

    public class RaisePolicyFactory : IRaisePolicyFactory
    {
        private readonly IPolicyService _policyService;
        private readonly IPlanService _planService;
        private readonly IPartyService _partyService;
        private readonly ICoverService _coverService;
        private readonly IRiskService _riskService;
        private readonly IOptionService _optionService;
        private readonly IPolicyOwnerService _policyOwnerService;

        private readonly IRaisePolicyRiskConverter _raisePolicyRiskConverter;
        private readonly IRaisePolicyPlanConverter _raisePolicyPlanConverter;
        private readonly IRaisePolicyCoverConverter _raisePolicyCoverConverter;
        private readonly IRaisePolicyOptionConverter _raisePolicyOptionConverter;
        private readonly IRaisePolicyBeneficiaryConverter _raisePolicyBeneficiaryConverter;
        private readonly IPolicyWithRisksService _policyWithRisksService;
        private readonly IPaymentOptionService _paymentOptionService;
        private readonly IRaisePaymentConverter _raisePaymentConverter;
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICompleteReferralService _completeReferralService;
        private readonly ICoverLoadingService _coverLoadingService;
        private readonly ICoverExclusionsService _coverExclusionsService;
        private readonly IPolicyDocumentUrlProvider _policyDocumentUrlProvider;

        public RaisePolicyFactory(IPolicyService policyService, IPlanService planService, IPartyService partyService,
            ICoverService coverService, IRiskService riskService, IRaisePolicyRiskConverter raisePolicyRiskConverter,
            IRaisePolicyPlanConverter raisePolicyPlanConverter, IRaisePolicyCoverConverter raisePolicyCoverConverter,
            IRaisePolicyBeneficiaryConverter raisePolicyBeneficiaryConverter, IPolicyWithRisksService policyWithRisksService, 
            IOptionService optionService, IRaisePolicyOptionConverter raisePolicyOptionConverter, 
            IPaymentOptionService paymentOptionService, IRaisePaymentConverter raisePaymentConverter,
            IPolicyPremiumCalculation policyPremiumCalculation, IDateTimeProvider dateTimeProvider, ICompleteReferralService completeReferralService, 
            ICoverLoadingService coverLoadingService, ICoverExclusionsService coverExclusionsService, IPolicyDocumentUrlProvider policyDocumentUrlProvider, 
            IPolicyOwnerService policyOwnerService)
        {
            _policyService = policyService;
            _planService = planService;
            _partyService = partyService;
            _coverService = coverService;
            _riskService = riskService;
            _raisePolicyRiskConverter = raisePolicyRiskConverter;
            _raisePolicyPlanConverter = raisePolicyPlanConverter;
            _raisePolicyCoverConverter = raisePolicyCoverConverter;
            _raisePolicyBeneficiaryConverter = raisePolicyBeneficiaryConverter;
            _policyWithRisksService = policyWithRisksService;
            _optionService = optionService;
            _raisePolicyOptionConverter = raisePolicyOptionConverter;
            _paymentOptionService = paymentOptionService;
            _raisePaymentConverter = raisePaymentConverter;
            _policyPremiumCalculation = policyPremiumCalculation;
            _dateTimeProvider = dateTimeProvider;
            _completeReferralService = completeReferralService;
            _coverLoadingService = coverLoadingService;
            _coverExclusionsService = coverExclusionsService;
            _policyDocumentUrlProvider = policyDocumentUrlProvider;
            _policyOwnerService = policyOwnerService;
        }

        public Models.RaisePolicy GetFromQuoteReference(string quoteReference)
        {
            var retVal = new Models.RaisePolicy();
            retVal.DocumentUrl = _policyDocumentUrlProvider.For(quoteReference);
            retVal.ReadyToSubmitDateTime = _dateTimeProvider.GetCurrentDateAndTime();
            retVal.LastCompletedReferralDateTime = SetLastCompletedReferralDateTime(quoteReference);
            retVal.QuoteReference = quoteReference;
            retVal.Payment = new RaisePolicyPayment();
            retVal.Risks = new List<RaisePolicyRisk>();
            
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);

            retVal.Id = policy.Id;
            retVal.ModifiedBy = policy.ModifiedBy;
            retVal.ModifiedTS = policy.ModifiedTS;
            retVal.Premium = policy.Premium;
            retVal.PremiumFrequency = policy.PremiumFrequency;
            retVal.BrandKey = policy.BrandKey;

            var risks = _policyService.GetRisksForPolicy(policy);
            
            foreach (var risk in risks)
            {
                var party = _partyService.GetParty(risk.PartyId);
                var rvRisk = _raisePolicyRiskConverter.From(risk, party);
                rvRisk.Plans = new List<RaisePolicyPlan>();

                var plans = _planService.GetPlansForRisk(risk.Id);
                foreach (var plan in plans)
                {
                    var parentPlan = _planService.GetParentPlanForPlan(plan, plans);

                    if (_planService.IsPlanSelected(plan, parentPlan))
                    {
                        var rvPlan = _raisePolicyPlanConverter.From(plan, policy.BrandKey);

                        var covers = _coverService.GetCoversForPlan(plan.Id);
                        rvPlan.Covers = covers.Select(c => _raisePolicyCoverConverter.From(c, _coverLoadingService.GetCoverLoadingsForCover(c), _coverExclusionsService.GetExclusionsForCover(c))).ToList();

                        rvPlan.Options = BuildOptions(plan, parentPlan);
                        
                        rvRisk.Plans.Add(rvPlan);
                    }
                }

                if (!risk.LprBeneficiary)
                {
                    var beneficiaries = _riskService.GetBeneficiariesForRisk(risk);
                    rvRisk.Beneficiaries = beneficiaries.Select(b => _raisePolicyBeneficiaryConverter.From(b)).ToList();
                }
                
                retVal.Risks.Add(rvRisk);
                
                //include first risk as a primary risk
                if (retVal.PrimaryRisk == null)
                {
                    retVal.PrimaryRisk = rvRisk;
                }
            }

            var ownerPartyId = _policyOwnerService.GetPolicyOwnerPartyId(policy.Id);
            var ownerParty = _partyService.GetParty(ownerPartyId.Value);
            var ownerFundName = _policyOwnerService.GetPolicyOwnerFundName(policy.Id);
            var ownerType = _policyOwnerService.GetPolicyOwnerType(policy.Id);
            retVal.Owner = _raisePolicyRiskConverter.From(ownerParty, ownerFundName, ownerType.Value);

            var payment = _paymentOptionService.GetCurrentPaymentOptions(quoteReference, retVal.PrimaryRisk.Id);

            retVal.Payment = _raisePaymentConverter.From(payment);

            if (retVal.PremiumFrequency != PremiumFrequency.Monthly)
            {
                ConvertPremiumsToMonthly(retVal);

                //TODO
                //because of our caching we need to change the premiums back to the original premium frequency or else we end up with cached objects different to the database
                //this is a less than ideal solution. Need to add IsolatedScope option to clone the policy, risks, plans, coveres and not mutate the cache object
                _policyPremiumCalculation.CalculateForPremiumFrequency(retVal.QuoteReference, retVal.PremiumFrequency);
            }

            return retVal;
        }

        private List<RaisePolicyOption> BuildOptions(IPlan plan, IPlan parentPlan)
        {
            var planOptions = _optionService.GetOptionsForPlan(plan.Id);
            var raiseOptions = planOptions.Select(_raisePolicyOptionConverter.From).ToList();

            if (parentPlan != null)
            {
                var parentPlanOptions = _optionService.GetOptionsForPlan(parentPlan.Id);

                var premiumReliefOption =
                    parentPlanOptions.FirstOrDefault(
                        o =>
                            (o.Code == ProductOptionConstants.LifePremiumRelief ||
                             o.Code == ProductOptionConstants.CiPremiumRelief ||
                             o.Code == ProductOptionConstants.TpdPremiumRelief));

                if (premiumReliefOption != null)
                {
                    raiseOptions.Add(_raisePolicyOptionConverter.From(premiumReliefOption));
                }
            }

            return raiseOptions;
        }

        private DateTime? SetLastCompletedReferralDateTime(string quoteReference)
        {
            var lastCompletedReferralDateTime = _completeReferralService.GetLastCompletedReferralDateTimeForPolicy(quoteReference);
            return lastCompletedReferralDateTime;
        }

        private void ConvertPremiumsToMonthly(Models.RaisePolicy retVal)
        {
            var monthlyCalculationResult  = _policyPremiumCalculation.CalculateForPremiumFrequency(retVal.QuoteReference, PremiumFrequency.Monthly);

            foreach (var risk in retVal.Risks)
            {
                var calcRisk = monthlyCalculationResult.RiskPremiums.FirstOrDefault(r => r.RiskId == risk.Id);

                if (calcRisk != null)
                {
                    foreach (var plan in risk.Plans)
                    {
                        var calcPlan = calcRisk.PlanPremiums.FirstOrDefault(p => p.PlanCode == plan.Code);

                        if (calcPlan != null)
                        {
                            foreach (var cover in plan.Covers)
                            {
                                var calcCover = calcPlan.CoverPremiums.FirstOrDefault(c => c.CoverCode == cover.Code);

                                if (calcCover != null)
                                {
                                    cover.Premium = calcCover.CoverPremium;
                                }
                            }

                            plan.Premium = calcPlan.PlanPremium;
                        }
                    }

                    risk.Premium = calcRisk.RiskPremium;
                }
            }

            retVal.Premium = monthlyCalculationResult.PolicyPremium;
        }
    }
}
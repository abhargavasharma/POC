using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TAL.QuoteAndApply.Customer.Web.Extensions;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;
using TAL.QuoteAndApply.Customer.Web.Services.Converters;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Models;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public enum ReviewWorkflowStatus
    {
        Accept,
        Refer
    }

    public interface ICustomerPolicyStateService
    {
        void EnsureSamePremiumTypesAcrossPlans(string quoteReference, int riskId);
        void FinaliseCustomerUnderwriting(string quoteReference, int riskId);
        void SyncWorkflowInterviewStatus(int riskId, ReviewWorkflowStatus reviewWorkflowStatus);
        PolicyReviewResponse GetRiskPolicyStatus(string quoteReference, int riskId);
        PolicyDetailsResponse GetPolicyDetails(string quoteReference);
    }

    public class CustomerPolicyStatusService : ICustomerPolicyStateService
    {
        private readonly IRiskUnderwritingService _riskUnderwritingService;
        private readonly IRiskUnderwritingQuestionService _riskUnderwritingQuestionService;
        private readonly IRiskUnderwritingAnswerSyncService _riskUnderwritingAnswerSyncService;
        private readonly IPlanDetailsService _planDetailsService;
        private readonly IUpdatePlanService _updatePlanService;
        private readonly IPlanOverviewResultProvider _planOverviewResultProvider;
        private readonly IPolicySatusProvider _policyStatusProvider;
        private readonly ILoadedQuestionPremiumCalculationService _loadedQuestionPremiumCalculation;
        private readonly IUnderwritingQuestionChoiceResponseConverter _choiceResponseConverter;
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;

        public CustomerPolicyStatusService(IRiskUnderwritingService riskUnderwritingService,
            IPlanDetailsService planDetailsService, IUpdatePlanService updatePlanService,
            IPlanOverviewResultProvider planOverviewResultProvider, IPolicySatusProvider policyStatusProvider,
            IRiskUnderwritingQuestionService riskUnderwritingQuestionService,
            ILoadedQuestionPremiumCalculationService loadedQuestionPremiumCalculation,
            IUnderwritingQuestionChoiceResponseConverter choiceResponseConverter,
            IRiskUnderwritingAnswerSyncService riskUnderwritingAnswerSyncService,
            IPolicyPremiumCalculation policyPremiumCalculation)
        {
            _riskUnderwritingService = riskUnderwritingService;
            _planDetailsService = planDetailsService;
            _updatePlanService = updatePlanService;
            _planOverviewResultProvider = planOverviewResultProvider;
            _policyStatusProvider = policyStatusProvider;
            _riskUnderwritingQuestionService = riskUnderwritingQuestionService;
            _loadedQuestionPremiumCalculation = loadedQuestionPremiumCalculation;
            _choiceResponseConverter = choiceResponseConverter;
            _riskUnderwritingAnswerSyncService = riskUnderwritingAnswerSyncService;
            _policyPremiumCalculation = policyPremiumCalculation;
        }

        public void EnsureSamePremiumTypesAcrossPlans(string quoteReference, int riskId)
        {
            var planDetails = _planOverviewResultProvider.GetFor(riskId).ToArray();
            var selectedPremiumType = planDetails.DefaultCustomerPremiumType();

            if (planDetails.Any(p => p.PremiumType != selectedPremiumType))
            {
                _updatePlanService.UpdatePremiumTypeOnAllPlans(quoteReference, riskId, selectedPremiumType);
            }
        }

        public void FinaliseCustomerUnderwriting(string quoteReference, int riskId)
        {
            //Sync customers underwriting
            _riskUnderwritingAnswerSyncService.SyncRiskWithFullInterviewAndUpdatePlanEligibility(riskId); //Re-sync EVERYTHING
            _policyPremiumCalculation.CalculateAndSavePolicy(quoteReference);

            //Set risk underwriting status in DB
            var riskPolicyStatus = GetRiskPolicyStatus(quoteReference, riskId);
            SyncWorkflowInterviewStatus(riskId, riskPolicyStatus.ReviewWorkflowStatus);
        }

        public void SyncWorkflowInterviewStatus(int riskId, ReviewWorkflowStatus reviewWorkflowStatus)
        {            
            switch (reviewWorkflowStatus)
            {
                case ReviewWorkflowStatus.Accept:
                    _riskUnderwritingService.SetRiskStatusToComplete(riskId);
                    break;
                case ReviewWorkflowStatus.Refer:
                    _riskUnderwritingService.SetRiskStatusToRefer(riskId);
                    break;

                default:
                    //Shouldn't even get into this state??
                    throw new InvalidEnumArgumentException($"Review workflow status '{reviewWorkflowStatus}' not handled");
            }
        }

        public PolicyReviewResponse GetRiskPolicyStatus(string quoteReference, int riskId)
        {
            var underwritingStatus = _riskUnderwritingService.GetRiskUnderwritingStatus(riskId);
            var planDetails = _planDetailsService.GetPlanDetailsForRisk(quoteReference, riskId);

            //TODO: The cover codes should be mapped to underwriting benefit codes.
            //Am being lazy here because we don't care about riders at the moment, main plan cover codes map to benefit codes directly.
            var allSelectedPlans = planDetails.Plans.Where(p => p.IsSelected);
            var allSelectedCoverCodes =
                allSelectedPlans.SelectMany(p => p.Covers.Where(c => c.IsSelected)).Select(c => c.Code).ToArray();

            var choicePointQuestionResponses = GetChoicePointQuestionsResponses(quoteReference, riskId,
                planDetails.TotalPremium, allSelectedCoverCodes);

            var sharedLoadingReponses = GetSharedLoadingResponses(quoteReference, riskId, planDetails.TotalPremium,
                allSelectedCoverCodes);

            return new PolicyReviewResponse(
                GetCustomerUnderwritingStatus(planDetails, underwritingStatus),
                planDetails,
                choicePointQuestionResponses,
                sharedLoadingReponses);
        }

        public PolicyDetailsResponse GetPolicyDetails(string quoteReference)
        {
            var policyDto = _policyStatusProvider.GetStatus(quoteReference);
            return new PolicyDetailsResponse
            {
                HasSaved = policyDto.SaveStatus != PolicySaveStatus.NotSaved,
                QuoteReference = policyDto.QuoteReferenceNumber,
                Source = policyDto.Source
            };
        }

        private IEnumerable<UnderwritingQuestionChoiceResponse> GetChoicePointQuestionsResponses(string quoteReference,
            int riskId, decimal totalPremium, IList<string> selectedCoverCodes)
        {
            //Get all choice questions per benefit
            var choicePointQuestionsPerCover =
                _riskUnderwritingQuestionService.GetChoicePointQuestions(riskId, selectedCoverCodes).ToArray();

            //Calculate diff in premium for each question choice
            var choicePointCalculationResults = _loadedQuestionPremiumCalculation.Calculate(quoteReference, riskId,
                totalPremium, choicePointQuestionsPerCover).ToArray();

            //Map choice point to responses
            var choicePointQuestionResponses = _choiceResponseConverter.From(choicePointQuestionsPerCover,
                choicePointCalculationResults);

            return choicePointQuestionResponses;
        }

        private IEnumerable<SharedLoadingResponse> GetSharedLoadingResponses(string quoteReference, int riskId,
            decimal totalPremium, IList<string> selectedCoverCodes)
        {
            var loadedQuesions = _riskUnderwritingQuestionService.GetQuestionsWithLoadings(riskId, selectedCoverCodes).ToArray();
            var loadingQuestionsCalculationResults = _loadedQuestionPremiumCalculation.Calculate(quoteReference, riskId,
                totalPremium, loadedQuesions);
            var sharedLoadingReponses = _choiceResponseConverter.ToLoadingResponses(loadedQuesions,
                loadingQuestionsCalculationResults);

            return sharedLoadingReponses;
        }

        private ReviewWorkflowStatus GetCustomerUnderwritingStatus(GetPlanResponse planResponse,
            RiskUnderwritingCompleteResult underwritingCompleteResult)
        {
            //TODO: keeping this logic here for now, cause that's where it is. But consider using exisitng rules/service for these checks
            if (planResponse.Plans.All(p => !p.Availability.IsAvailable))
            {
                return ReviewWorkflowStatus.Refer;
            }

            if (underwritingCompleteResult.IsUnderwritingCompleteForRisk)
            {
                return ReviewWorkflowStatus.Accept;
            }

            if (underwritingCompleteResult.CoverUnderwritingCompleteResults.Any(
                    r => r.ValidationType == ValidationType.Error || r.ValidationType == ValidationType.Warning))
            {
                return ReviewWorkflowStatus.Refer;
            }

            throw new ApplicationException("Didn't determine Customer Underwriting Status");
        }
    }
}
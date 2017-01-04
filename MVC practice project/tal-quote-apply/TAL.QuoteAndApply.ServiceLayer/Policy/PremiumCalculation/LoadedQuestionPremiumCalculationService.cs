using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation
{

    public class LoadedQuestionPremiumCalculationResult
    {
        public LoadedQuestionPremiumCalculationResult(UnderwritingQuestion question, decimal premiumDiff)
        {
            PremiumDiff = premiumDiff;
            Question = question;
        }

        public UnderwritingQuestion Question { get; private set; }
        public decimal PremiumDiff { get; private set; }
    }

    public interface ILoadedQuestionPremiumCalculationService
    {
        IEnumerable<LoadedQuestionPremiumCalculationResult> Calculate(string quoteReference, int riskId, decimal currentTotalPremium,
            IEnumerable<UnderwritingQuestionsByBenefitCode> choiceQuestionsPerBenefit);
    }

    public class LoadedQuestionPremiumCalculationService : ILoadedQuestionPremiumCalculationService
    {
        private readonly IPolicyPremiumCalculation _policyPremiumCalculation;

        public LoadedQuestionPremiumCalculationService(IPolicyPremiumCalculation policyPremiumCalculation)
        {
            _policyPremiumCalculation = policyPremiumCalculation;
        }

        public IEnumerable<LoadedQuestionPremiumCalculationResult> Calculate(string quoteReference, int riskId, decimal currentTotalPremium,
            IEnumerable<UnderwritingQuestionsByBenefitCode> choiceQuestionsPerBenefit)
        {
            //Only need to calculate once per question
            var uniqueQuestions = choiceQuestionsPerBenefit.SelectMany(qpb => qpb.Questions).DistinctBy(q => q.Id);

            foreach (var question in uniqueQuestions)
            {
                //Only calculate if answered with a loading 
                var selectedLoadingAnswer = question.Answers.SingleOrDefault(a => a.IsSelected && (a.Loadings?.Any() ?? false));
                if (selectedLoadingAnswer == null)
                {
                    yield return new LoadedQuestionPremiumCalculationResult(question, 0);
                    continue;
                }
                
                var coversAffected = choiceQuestionsPerBenefit.Where(qpb => qpb.Questions.Any(q => q.Id == question.Id)).Select(qpb => qpb.BenefitCode);

                var loading = selectedLoadingAnswer.Loadings.First(); //TODO: assuming only one loading applies per answer
                var loadingAdjustment = PremiumCalcLoadingAdjuster.ReduceBy(loading, coversAffected);

                var calculateResult = _policyPremiumCalculation.Calculate(quoteReference, loadingAdjustment);

                var diff = currentTotalPremium - calculateResult.RiskPremiumCalculationResults.Single(r => r.RiskId == riskId).RiskPremium;
                yield return new LoadedQuestionPremiumCalculationResult(question, diff);
            }
        }
    }
}

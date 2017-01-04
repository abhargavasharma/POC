using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Policy.PremiumCalculation;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;

namespace TAL.QuoteAndApply.Customer.Web.Services.Converters
{
    public interface IUnderwritingQuestionChoiceResponseConverter
    {
        IEnumerable<UnderwritingQuestionChoiceResponse> From(
            IList<UnderwritingQuestionsByBenefitCode> questionsByBenefitCode,
            IEnumerable<LoadedQuestionPremiumCalculationResult> choicePointCalculationResults);

        IEnumerable<SharedLoadingResponse> ToLoadingResponses(
            IList<UnderwritingQuestionsByBenefitCode> questionsByBenefitCode,
            IEnumerable<LoadedQuestionPremiumCalculationResult> loadedQuestionCalculationResults);
    }

    public class UnderwritingQuestionChoiceResponseConverter : IUnderwritingQuestionChoiceResponseConverter
    {
        private readonly IProductDefinitionProvider _productDefinitionProvider;
        private readonly ICurrentProductBrandProvider _currentProductBrandProvider;

        public UnderwritingQuestionChoiceResponseConverter(IProductDefinitionProvider productDefinitionProvider, ICurrentProductBrandProvider currentProductBrandProvider)
        {
            _productDefinitionProvider = productDefinitionProvider;
            _currentProductBrandProvider = currentProductBrandProvider;
        }

        public IEnumerable<UnderwritingQuestionChoiceResponse> From(IList<UnderwritingQuestionsByBenefitCode> questionsByBenefitCode,
            IEnumerable<LoadedQuestionPremiumCalculationResult> choicePointCalculationResults)
        {

            foreach (var choicePointCalculationResult in choicePointCalculationResults)
            {

                var applicableBenefitsNames =
                    questionsByBenefitCode.Where(cpq => cpq.Questions.Any(q => q.Id == choicePointCalculationResult.Question.Id)).Select(cpq => cpq.BenefitCode).ToArray();

                //Get loading/exclusion answer detail and map to return object.                
                var exclusionAnswer = choicePointCalculationResult.Question.Answers.Single(a => a.Exclusions.Any());
                var loadingAnswer = choicePointCalculationResult.Question.Answers.Single(a => a.Loadings.Any());

                var questionChoiceType = UnderwritingQuestionChoiceType.None;
                var answerId = "";
                var name = "";

                if (exclusionAnswer.IsSelected)
                {
                    questionChoiceType = UnderwritingQuestionChoiceType.Exclusion;
                    answerId = loadingAnswer.Id;
                    name = exclusionAnswer.Exclusions.First().Name; //TODO: Assuming one exclusion
                }
                if (loadingAnswer.IsSelected)
                {
                    questionChoiceType = UnderwritingQuestionChoiceType.Loading;
                    answerId = exclusionAnswer.Id;
                    name = loadingAnswer.Loadings.First().Name; //TODO: Assuming one loading
                }

                if (questionChoiceType == UnderwritingQuestionChoiceType.None)
                {
                    throw new ApplicationException("Question choice not determined"); //Just guard against not setting question choice properly
                }

                var currentBrand = _currentProductBrandProvider.GetCurrent();

                yield return new UnderwritingQuestionChoiceResponse(questionChoiceType, name,
                    choicePointCalculationResult.PremiumDiff,
                    applicableBenefitsNames.ToList(),
                    _productDefinitionProvider.GetParentPlanCodes(applicableBenefitsNames, currentBrand.BrandCode).ToList(),
                    choicePointCalculationResult.Question.Id, answerId);
            }

        }

        public IEnumerable<SharedLoadingResponse> ToLoadingResponses(
            IList<UnderwritingQuestionsByBenefitCode> questionsByBenefitCode,
            IEnumerable<LoadedQuestionPremiumCalculationResult> loadedQuestionCalculationResults)
        {
            foreach (var loadedQuestionCalculationResult in loadedQuestionCalculationResults)
            {
                var applicableBenefitsNames =
                    questionsByBenefitCode.Where(
                        cpq => cpq.Questions.Any(q => q.Id == loadedQuestionCalculationResult.Question.Id))
                        .Select(cpq => cpq.BenefitCode)
                        .ToArray();

                //Get loading answer detail and map to return object.                
                var loadingAnswer = loadedQuestionCalculationResult.Question.Answers.Single(a => a.IsSelected && (a.Loadings?.Any() ?? false));
                var loading = loadingAnswer.Loadings.First(); //Assumption that only one loading applies per answer

                var currentBrand = _currentProductBrandProvider.GetCurrent();

                yield return
                    new SharedLoadingResponse(loading.Name, loadedQuestionCalculationResult.PremiumDiff,
                        _productDefinitionProvider.GetParentPlanCodes(applicableBenefitsNames, currentBrand.BrandCode));
            }

        }
    }
}
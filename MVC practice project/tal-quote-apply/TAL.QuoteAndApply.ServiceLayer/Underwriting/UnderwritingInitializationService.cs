using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Underwriting.Configuration;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface IUnderwritingInitializationService
    {
        InterviewReferenceInformation CreateInterviewAndAnswerRatingFactors(RatingFactorsParam ratingFactorsParam);
    }

    public class UnderwritingInitializationService : IUnderwritingInitializationService
    {
        private readonly IUnderwritingConfigurationProvider _underwritingConfigurationProvider;
        private readonly IUnderwritingTemplateService _underwritingTemplateService;
        private readonly ICreateUnderwritingInterview _createUnderwritingInterview;
        private readonly IUnderwritingRatingFactorsService _underwritingRatingFactorsService;

        public UnderwritingInitializationService(
            IUnderwritingConfigurationProvider underwritingConfigurationProvider,
            IUnderwritingTemplateService underwritingTemplateService,
            ICreateUnderwritingInterview createUnderwritingInterview,
            IUnderwritingRatingFactorsService underwritingRatingFactorsService)
        {
            _underwritingConfigurationProvider = underwritingConfigurationProvider;
            _underwritingTemplateService = underwritingTemplateService;
            _createUnderwritingInterview = createUnderwritingInterview;
            _underwritingRatingFactorsService = underwritingRatingFactorsService;
        }

        private ReadOnlyUnderwritingInterview CreateInterview()
        {
            var templateName = _underwritingConfigurationProvider.TemplateName;
            var workflow = _underwritingConfigurationProvider.FullWorkflow;

            var templateInfo = _underwritingTemplateService.GetTemplateInformation(templateName);
            return _createUnderwritingInterview.CreateNewInterview(templateName, workflow, templateInfo.Benefits);
        }

        public InterviewReferenceInformation CreateInterviewAndAnswerRatingFactors(RatingFactorsParam ratingFactorsParam)
        {
            var interview = CreateInterview();
            return _underwritingRatingFactorsService.UpdateUnderwritingWithRatingFactorValues(interview.InterviewIdentifier, interview.TemplateVersion, interview.ConcurrencyToken, ratingFactorsParam);
        }
    }
}
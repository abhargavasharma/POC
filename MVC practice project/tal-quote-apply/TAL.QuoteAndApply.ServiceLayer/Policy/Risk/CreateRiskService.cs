using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk
{
    public interface ICreateRiskService
    {
        CreateRiskResult CreateRisk(IPolicy policy, IParty party, RatingFactorsParam ratingFactorsParam);
    }

    public class CreateRiskService : ICreateRiskService
    {
        private readonly IRiskDtoConverter _riskDtoConverter;
        private readonly IRiskService _riskService;
        private readonly IUnderwritingInitializationService _underwritingInitializationService;
        private readonly IRiskMarketingStatusUpdater _riskMarketingStatusUpdater;

        public CreateRiskService(IRiskDtoConverter riskDtoConverter, 
            IRiskService riskService, 
            IUnderwritingInitializationService underwritingInitializationService, 
            IRiskMarketingStatusUpdater riskMarketingStatusUpdater)
        {
            _riskDtoConverter = riskDtoConverter;
            _riskService = riskService;
            _underwritingInitializationService = underwritingInitializationService;
            _riskMarketingStatusUpdater = riskMarketingStatusUpdater;
        }

        public CreateRiskResult CreateRisk(IPolicy policy, IParty party, RatingFactorsParam ratingFactorsParam)
        {
            //create interview
            // ASSUMPTIONS - Client does not already exist
            var interview = _underwritingInitializationService.CreateInterviewAndAnswerRatingFactors(ratingFactorsParam);

            //create risk
            var riskDto = _riskDtoConverter.CreateFrom(policy, party, ratingFactorsParam, interview);
            var risk = _riskService.CreateRisk(riskDto);
            _riskMarketingStatusUpdater.CreateNewRiskMarketingStatus(risk, DataModel.Policy.MarketingStatus.Accept);

            return new CreateRiskResult {Risk = risk, InterviewReferenceInformation = interview};
        }
    }
}
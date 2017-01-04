using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Plan.Models.Mappers;
using TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Product;
using ICoverService = TAL.QuoteAndApply.Policy.Service.ICoverService;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface ICreatePlanService
    {
        IPlan CreatePlan(PlanStateParam quoteOptionsParam);
        IPlan CreateRider(PlanStateParam planOptionsParam, int parentPlanId);
    }

    public class CreatePlanService : ICreatePlanService
    {
        private readonly IPlanService _planService;
        private readonly ICoverService _coverService;
        private readonly IOptionService _optionService;
        private readonly IPlanDtoConverter _planDtoConverter;
        private readonly ICoverDtoConverter _coverDtoConverter;
        private readonly IOptionDtoConverter _optionDtoConverter;
        private readonly IProductDefinitionProvider _productDefinitionBuilder;
        private readonly IPlanMarketingStatusUpdater _planMarketingStatusUpdater;
        private readonly ICoverMarketingStatusUpdater _coverMarketingStatusUpdater;

        public CreatePlanService(IPlanService planService, IPlanDtoConverter planDtoConverter, 
            ICoverDtoConverter coverDtoConverter, IProductDefinitionProvider productDefinitionBuilder, IOptionService optionService, 
            IOptionDtoConverter optionDtoConverter, ICoverService coverService, IPlanMarketingStatusUpdater planMarketingStatusUpdater,
            ICoverMarketingStatusUpdater coverMarketingStatusUpdater)
        {
            _planService = planService;
            _planDtoConverter = planDtoConverter;
            _coverDtoConverter = coverDtoConverter;
            _productDefinitionBuilder = productDefinitionBuilder;
            _optionService = optionService;
            _optionDtoConverter = optionDtoConverter;
            _coverService = coverService;
            _planMarketingStatusUpdater = planMarketingStatusUpdater;
            _coverMarketingStatusUpdater = coverMarketingStatusUpdater;
        }

        public IPlan CreatePlan(PlanStateParam planOptionsParam)
        {
            var planDto = _planDtoConverter.CreateFrom(planOptionsParam);
            return CreatePlanCoversAndOptions(planDto, planOptionsParam);
        }


        public IPlan CreateRider(PlanStateParam planOptionsParam, int parentPlanId)
        {
            var planDto = _planDtoConverter.CreateFrom(planOptionsParam, parentPlanId);
            return CreatePlanCoversAndOptions(planDto, planOptionsParam);
        }

        private IPlan CreatePlanCoversAndOptions(PlanDto planDto, PlanStateParam planOptionsParam)
        {
            //create plan
            var returnPlan = _planService.CreatePlan(planDto);

            _planMarketingStatusUpdater.CreateNewPlanMarketingStatus(returnPlan, planOptionsParam.MarketingStatus);

            var planDefinition = _productDefinitionBuilder.GetPlanDefinition(planOptionsParam.PlanCode, planOptionsParam.BrandKey);
            
            foreach (var cover in planDefinition.Covers)
            {
                //create cover object - Assumption no covers are selected when plan created hence the false
                var coverDto =
                    _coverDtoConverter.CreateFrom(planOptionsParam, false, returnPlan.Id, cover.Code);

                //create cover
                _coverService.CreateCover(coverDto);
                _coverMarketingStatusUpdater.CreateNewCoverMarketingStatus(coverDto);
            }

            foreach (var option in planDefinition.Options)
            {
                //create option object - Assumption no options are selected when plan created hence the false
                var optionDto =
                    _optionDtoConverter.CreateFrom(planOptionsParam, option.Selected, returnPlan.Id, option.Code);

                //create option
                _optionService.CreateOption(optionDto);
            }

            return returnPlan;
        }
    }
}

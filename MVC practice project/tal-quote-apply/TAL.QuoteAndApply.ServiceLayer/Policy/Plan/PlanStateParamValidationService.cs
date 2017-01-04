using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface IPlanStateParamValidationService
    {
        IEnumerable<ValidationError> ValidatePlanStateParam(PlanStateParam planOptionsParam);
    }

    public class PlanStateParamValidationService : IPlanStateParamValidationService
    {
        private readonly IProductRulesService _productRulesService;
        private readonly IPlanRulesService _planRulesService;

        public PlanStateParamValidationService(IProductRulesService productRulesService, IPlanRulesService planRulesService)
        {
            _productRulesService = productRulesService;
            _planRulesService = planRulesService;
        }

        public IEnumerable<ValidationError> ValidatePlanStateParam(PlanStateParam planOptionsParam)
        {
            var errors = new List<ValidationError>();

            errors.AddRange(_productRulesService.ValidatePlanStateParam(planOptionsParam));
            errors.AddRange(_planRulesService.ValidatePlanStateParam(planOptionsParam));

            return errors;
        }
    }
}

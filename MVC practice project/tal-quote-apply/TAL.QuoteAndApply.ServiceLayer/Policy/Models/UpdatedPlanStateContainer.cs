using System.Collections.Generic;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class UpdatedPlanStateContainer
    {
        public UpdatedPlanStateContainer(PlanStateParam updatedPlanState, IEnumerable<AvailableFeature> changedFeatures)
        {
            UpdatedPlanState = updatedPlanState;
            ChangedFeatures = changedFeatures;
        }

        public PlanStateParam UpdatedPlanState { get; }
        public IEnumerable<AvailableFeature> ChangedFeatures { get; }
    }
}
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.Models
{
    public class AvailablePlanOptionsAndConfigResult
    {
        public AvailablePlanOptionsAndConfigResult(string planCode, IEnumerable<string> availableOptions,
            IEnumerable<string> availableCovers, IEnumerable<string> availablePlans,
            IEnumerable<AvailableRiderOptionsAndConfigResult> availableRiders,
            IEnumerable<AvailableFeature> unavailableFeatures, IEnumerable<AvailableFeature> variableAvailability)
        {
            PlanCode = planCode;
            UnavailableFeatures = unavailableFeatures;
            VariableAvailability = variableAvailability;
            AvailableCovers = availableCovers.ToList();
            AvailablePlans = availablePlans.ToList();
            AvailableRiders = availableRiders.ToList();
            AvailableOptions = availableOptions.ToList();
        }

        public string PlanCode { get; private set; }
        public IReadOnlyList<string> AvailablePlans { get; private set; }
        public IReadOnlyList<string> AvailableCovers { get; private set; }

        public IReadOnlyList<AvailableRiderOptionsAndConfigResult> AvailableRiders { get; private set; }

        public IReadOnlyList<string> AvailableOptions { get; private set; }

        public IEnumerable<AvailableFeature> UnavailableFeatures { get; private set; }
        public IEnumerable<AvailableFeature> VariableAvailability { get; private set; }
    }
}
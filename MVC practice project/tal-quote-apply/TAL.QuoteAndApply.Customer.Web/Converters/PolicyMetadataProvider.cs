using Newtonsoft.Json.Linq;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.Converters
{
    public interface IPolicyInitialisationMetadataProvider
    {
        PolicyInitialisationMetadata GetPolicyInitialisationMetadata(EntryPointViewModel entryPointViewModel);
        PolicyInitialisationMetadata GetPolicyInitialisationMetadata(CalculatorResults entryPointViewModel);
    }

    public class PolicyInitialisationMetadataProvider : IPolicyInitialisationMetadataProvider
    {
        public PolicyInitialisationMetadata GetPolicyInitialisationMetadata(EntryPointViewModel entryPointViewModel)
        {
            return new PolicyInitialisationMetadata(
                entryPointViewModel.ContactId, 
                entryPointViewModel.CalculatorResultsJson, 
                entryPointViewModel.CalculatorAssumptionsJson, 
                false,
                true);
        }

        public PolicyInitialisationMetadata GetPolicyInitialisationMetadata(CalculatorResults results)
        {
           return new PolicyInitialisationMetadata(
               "1",
               JObject.Parse(results.Results), 
               JObject.Parse(results.Assumptions), 
               false, 
               results.UseResultConfirmationRequired);
        }
    }
}
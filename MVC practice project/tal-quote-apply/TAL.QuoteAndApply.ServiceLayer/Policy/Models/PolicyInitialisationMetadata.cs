using Newtonsoft.Json.Linq;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyInitialisationMetadata
    {
        public string ContactId { get; private set; }

        public JObject CalculatorResultsJson { get; private set; }
        public JObject CalculatorAssumptionsJson { get; private set; }

        public bool CalculatorResultsUsed { get; private set; }

        public bool UseResultConfirmationRequired { get; private set; }

        public PolicyInitialisationMetadata(string contactId, 
                                            JObject calculatorResultsJson, 
                                            JObject calculatorAssumptionsJson, 
                                            bool calculatorResultsUsed,
                                            bool useResultConfirmationRequired)
        {
            ContactId = contactId;
            CalculatorResultsJson = calculatorResultsJson;
            CalculatorAssumptionsJson = calculatorAssumptionsJson;
            CalculatorResultsUsed = calculatorResultsUsed;
            UseResultConfirmationRequired = useResultConfirmationRequired;
        }

        public void SetResultsUsed()
        {
            CalculatorResultsUsed = true;
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TAL.QuoteAndApply.Customer.Web.Models.View
{
    public class EntryPointViewModel
    {
        [Obsolete]
        public string ContactId { get; set; }
        public JObject CalculatorResultsJson { get; set; }
        public JObject CalculatorAssumptionsJson { get; set; }
    }
}
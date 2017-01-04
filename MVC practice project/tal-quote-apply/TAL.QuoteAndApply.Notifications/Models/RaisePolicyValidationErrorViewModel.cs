using System.Collections.Generic;

namespace TAL.QuoteAndApply.Notifications.Models
{
    public class RaisePolicyValidationErrorViewModel
    {
        public string QuoteReferenceNumber { get; set; }
        public IEnumerable<string> Validations { get; set; }
        public string Environment { get; set; }
    }
}
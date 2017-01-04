using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAL.QuoteAndApply.Customer.Web.Models
{
    public class ChatAvailabilityResponse
    {
        public bool WebChatIsAvailable { get; set; }
        public string WebChatUrl { get; set; }
        public string WebChatAvailableFrom { get; set; }
        public string WebChatAvailableTo { get; set; }
    }
}
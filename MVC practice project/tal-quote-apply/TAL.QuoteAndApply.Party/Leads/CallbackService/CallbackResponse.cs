using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TAL.QuoteAndApply.Party.Leads.CallbackService
{
    [XmlRoot("response", Namespace = "urn:tal:notificationLogRcp")]
    public class CallbackResponse
    {
        [XmlAttribute("status")]
        public string Status { get; set; }

        [XmlAttribute("errorCode")]
        public int ErrorCode { get; set; }
    }
}

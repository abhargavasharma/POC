using System.Collections.Generic;
using System.Xml.Serialization;

namespace TAL.QuoteAndApply.Party.Leads.LeadsService
{
    [XmlRoot("response", Namespace = "urn:nms:recipient")]
    public class LeadResponse
    {
        [XmlAttribute("status")]
        public string Status { get; set; }

        [XmlAttribute("errorCode")]
        public int ErrorCode { get; set; }

        [XmlAttribute("adobeId")]
        public int AdobeId { get; set; }
    }

    [XmlRoot("recipient-collection", Namespace = "urn:nms:recipient")]
    public class CommunicationPreferenceUpdateResponse
    {
        public List<RecipientUpdateResponse> RecipientUpdateResponses { get; set; }
    }

    [XmlRoot("recipient")]
    public class RecipientUpdateResponse
    {
        [XmlAttribute("adobeId")]
        public int AdobeId { get; set; }

        [XmlAttribute("updateStatus")]
        public string UpdateStatus { get; set; }

        [XmlAttribute("errorCode")]
        public int ErrorCode { get; set; }
    }
}
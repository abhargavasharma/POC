using System;
using System.Xml;

namespace TAL.QuoteAndApply.Party.Leads.LeadsService
{
    public static class LeadsServiceMessageHeaderBuilder
    {
        public static MessageHeader_Type GetMessageHeader()
        {
            return new MessageHeader_Type
            {
                ACORDStandardVersionCode = new ACORDStandardVersionCode_Type { Value = new XmlQualifiedName("AML_1_1_0") },
                MessageId = Guid.NewGuid().ToString(),
                CorrelationId = Guid.NewGuid().ToString(),
                MessageDateTime = DateTime.Now,
                Sender = new Sender_Type() { key = "TAL" },
                Receiver = new Receiver_Type() { key = "Adobe" },
                SendingSystem = new EndPointSystem_Type() { VendorProductName = "Leads" }
            };
        }
    }
}
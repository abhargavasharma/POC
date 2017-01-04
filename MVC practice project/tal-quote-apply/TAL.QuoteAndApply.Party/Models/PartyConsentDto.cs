
using System;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Party.Models
{
    public class PartyConsentDto : DbItem, IPartyConsent
    {
        public int PartyId { get; set; }
        public bool ExpressConsent { get; set; }
        public DateTime? ExpressConsentUpdatedTs { get; set; }
        public bool DncMobile { get; set; }
        public bool DncHomeNumber { get; set; }
        public bool DncEmail { get; set; }
        public bool DncPostalMail { get; set; }

        public PartyConsentDto() { }

        public PartyConsentDto(int partyId)
        {
            PartyId = partyId;
        }
    }
}


using System;

namespace TAL.QuoteAndApply.Party.Models
{
    public interface IPartyConsent
    {
        int Id { get; set; }
        int PartyId { get; set; }
        bool ExpressConsent { get; set; }
        DateTime? ExpressConsentUpdatedTs { get; set; }
        bool DncMobile { get; set; }
        bool DncHomeNumber { get; set; }
        bool DncEmail { get; set; }
        bool DncPostalMail { get; set; }
    }
}
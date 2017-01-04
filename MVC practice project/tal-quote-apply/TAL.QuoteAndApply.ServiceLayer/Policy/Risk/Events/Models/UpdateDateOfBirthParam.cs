using System;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models
{
    public class UpdateDateOfBirthParam
    {
        public int RiskId { get; private set; }
        public int PartyId { get; private set; }
        public DateTime DateOfBirth { get; private set; }

        public UpdateDateOfBirthParam(int riskId, int partyId, DateTime dateOfBirth)
        {
            RiskId = riskId;
            PartyId = partyId;
            DateOfBirth = dateOfBirth;
        }
    }
}

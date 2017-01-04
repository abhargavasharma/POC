namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models
{
    public class UpdateGenderParam
    {
        public int RiskId { get; private set; }
        public int PartyId { get; private set; }
        public char Gender { get; private set; }

        public UpdateGenderParam(int riskId, int partyId, char gender)
        {
            RiskId = riskId;
            PartyId = partyId;
            Gender = gender;
        }
    }
}
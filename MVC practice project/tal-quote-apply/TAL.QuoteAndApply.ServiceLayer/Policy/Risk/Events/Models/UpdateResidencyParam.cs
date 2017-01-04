using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models
{
    public class UpdateResidencyParam
    {
        public int RiskId { get; private set; }
        public int PartyId { get; private set; }
        public ResidencyStatus ResidencyStatus { get; private set; }

        public UpdateResidencyParam(int riskId, int partyId, ResidencyStatus residencyStatus)
        {
            RiskId = riskId;
            PartyId = partyId;
            ResidencyStatus = residencyStatus;
        }
    }
}
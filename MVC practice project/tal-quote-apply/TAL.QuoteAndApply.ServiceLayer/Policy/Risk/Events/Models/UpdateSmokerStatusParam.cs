using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models
{
    public class UpdateSmokerStatusParam
    {
        public int RiskId { get; private set; }
        public int PartyId { get; private set; }
        public SmokerStatus SmokerStatus { get; private set; }

        public UpdateSmokerStatusParam(int riskId, int partyId, SmokerStatus smokerStatus)
        {
            RiskId = riskId;
            PartyId = partyId;
            SmokerStatus = smokerStatus;
        }
    }
}
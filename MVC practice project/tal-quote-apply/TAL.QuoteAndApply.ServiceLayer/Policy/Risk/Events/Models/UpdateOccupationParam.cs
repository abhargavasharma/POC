namespace TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Events.Models
{
    public class UpdateOccupationParam
    {
        public int RiskId { get; private set; }
        public int PartyId { get; private set; }
        public string OccupationClass { get; private set; }
        public string OccupationCode { get; private set; }
        public string OccupationTitle { get; private set; }
        public string IndustryCode { get; private set; }
        public string IndustryTitle { get; private set; }
        public bool IsTpdAny { get; private set; }
        public bool IsTpdOwn { get; private set; }
        public decimal? TpdLoading { get; private set; }
        public string PasCode { get; private set; }

        public UpdateOccupationParam(int riskId, int partyId, string occupationClass, string occupationCode, string occupationTitle, string industryCode, string industryTitle, bool isTpdAny, bool isTpdOwn, decimal? tpdLoading, string pasCode)
        {
            RiskId = riskId;
            PartyId = partyId;
            OccupationClass = occupationClass;
            OccupationCode = occupationCode;
            OccupationTitle = occupationTitle;
            IndustryCode = industryCode;
            IndustryTitle = industryTitle;
            IsTpdAny = isTpdAny;
            IsTpdOwn = isTpdOwn;
            TpdLoading = tpdLoading;
            PasCode = pasCode;
        }

        public static UpdateOccupationParam NoOccupation(int riskId, int partyId)
        {
            return new UpdateOccupationParam(riskId, partyId, null, null, null, null, null, false, false, null, null);
        }
    }
}
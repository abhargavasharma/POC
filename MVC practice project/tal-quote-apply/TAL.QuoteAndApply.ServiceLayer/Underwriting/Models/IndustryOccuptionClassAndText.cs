using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Models
{
    public class IndustryOccuptionClassAndText
    {
        public string IndustryCode { get; private set; }
        public string IndustryText { get; private set; }
        public string OccupationText { get; private set; }
        public string OccupationClass { get; private set; }
        public string PasCode { get; private set; }
        public bool IsTpdOwn { get; private set; }
        public bool IsTpdAny { get; private set; }
        public decimal? TpdLoading { get; private set; }

        public IndustryOccuptionClassAndText(string occupationText, string occupationClass, string industryText, string industryCode, string pasCode, bool isTpdOwn, bool isTpdAny, decimal? tpdLoading)
        {
            OccupationText = occupationText;
            OccupationClass = occupationClass;
            PasCode = pasCode;
            IsTpdOwn = isTpdOwn;
            IsTpdAny = isTpdAny;
            TpdLoading = tpdLoading;
            IndustryCode = industryCode;
            IndustryText = industryText;
        }
    }
}

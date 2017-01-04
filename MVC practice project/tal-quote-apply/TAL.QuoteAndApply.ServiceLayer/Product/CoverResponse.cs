
using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public class CoverResponse
    {
        public string CoverType { get; set; }
        
        public decimal Premium { get; set; }
        
        public bool Rider { get; set; }
        
        public bool Selected { get; set; }
        
        public string Name { get; set; }
        
        public int CoverAmount { get; set; }

        public string CoverFor { get; set; }

        public string Code { get; set; }
        public string UnderwritingBenefitCode { get; set; }
        public CoverUnderwritingStatus CoverUnderwritingStatus { get; set; }
        public IEnumerable<ExclusionResponse> Exclusions { get; set; }

    }

    public enum CoverUnderwritingStatus
    {
        Accept = 1,
        Decline = 2,
        Defer = 3,
        Incomplete = 4,
        MoreInfo = 5,
        Refer = 6
    }
}
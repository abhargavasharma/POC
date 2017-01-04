using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models
{
    public class RaisePolicyCover : ICover
    {
        public int CoverAmount { get; set; }
        public int PolicyId { get; set; }
        public int RiskId { get; set; }
        public string Code { get; set; }
        public bool Selected { get; set; }
        public int PlanId { get; set; }
        public decimal Premium { get; set; }
        public UnderwritingStatus UnderwritingStatus { get; set; }
        public int Id { get; set; }

        public IList<RaisePolicyCoverExclusion> Exclusions { get; set; }
        public IList<RaisePolicyCoverLoading> Loadings { get; set; }

        public RaisePolicyCover()
        {
            Exclusions = new List<RaisePolicyCoverExclusion>();
            Loadings = new List<RaisePolicyCoverLoading>();
        }
    }

    public class RaisePolicyCoverExclusion : ICoverExclusion
    {
        public int CoverId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
    }

    public class RaisePolicyCoverLoading : ICoverLoading
    {
        public int CoverId { get; set; }
        public LoadingType LoadingType { get; set; }
        public decimal Loading { get; set; }
    }
}